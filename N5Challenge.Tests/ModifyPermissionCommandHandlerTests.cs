using Moq;
using N5Challenge.Handlers;
using N5Challenge.Commands;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Services.Interfaces;

public class ModifyPermissionCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesPermission_WhenPermissionAndTypeExist()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockPermissionTypeRepo = new Mock<IPermissionTypeRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();
        var permissionType = new PermissionType { Id = 1, Description = "Test" };
        var permission = new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionTypeId = 1, PermissionType = permissionType, PermissionDate = System.DateTime.Now };
        
        mockPermissionRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(permission);
        mockPermissionTypeRepo.Setup(r => r.GetByidAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(permissionType);
        mockUnitOfWork.Setup(u => u.PermissionRepository).Returns(mockPermissionRepo.Object);
        mockUnitOfWork.Setup(u => u.PermissionTypeRepository).Returns(mockPermissionTypeRepo.Object);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);

        var handler = new ModifyPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);
        var command = new ModifyPermissionCommand(1, "Jane", "Smith", 1, permission.PermissionDate);
        
        await handler.Handle(command, CancellationToken.None);

        mockPermissionRepo.Verify(r => r.Update(It.Is<Permission>(p => p.EmployeeForename == "Jane" && p.EmployeeSurname == "Smith")), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockKafkaService.Verify(k => k.Send(It.Is<KafkaMessageDto>(m => m.OperationName == OperationEnum.modify), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsArgumentException_WhenPermissionNotFound()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();
        
        mockPermissionRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Permission)null!);
        mockUnitOfWork.Setup(u => u.PermissionRepository).Returns(mockPermissionRepo.Object);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);
        
        var handler = new ModifyPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);
        var command = new ModifyPermissionCommand(99, "Jane", "Smith", 1, DateTime.Now);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }
} 