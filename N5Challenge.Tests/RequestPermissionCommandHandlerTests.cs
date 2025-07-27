using Moq;
using N5Challenge.Handlers;
using N5Challenge.Commands;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Services.Interfaces;

public class RequestPermissionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPermissionDto_WhenPermissionTypeExists()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionTypeRepo = new Mock<IPermissionTypeRepository>();
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();

        var permissionType = new PermissionType { Id = 1, Description = "Test" };
        var permission = new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionTypeId = 1, PermissionType = permissionType, PermissionDate = System.DateTime.Now };
        
        mockPermissionTypeRepo.Setup(r => r.GetByidAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(permissionType);
        mockPermissionRepo.Setup(r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>())).ReturnsAsync(permission);
        mockUnitOfWork.Setup(u => u.PermissionTypeRepository).Returns(mockPermissionTypeRepo.Object);
        mockUnitOfWork.Setup(u => u.PermissionRepository).Returns(mockPermissionRepo.Object);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);

        var handler = new RequestPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);
        var command = new RequestPermissionCommand("John", "Doe", 1, permission.PermissionDate);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("John", result.EmployeeForename);
        Assert.Equal("Doe", result.EmployeeSurname);
        Assert.Equal("Test", result.PermissionType);
        mockKafkaService.Verify(k => k.Send(It.Is<KafkaMessageDto>(m => m.OperationName == OperationEnum.request), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsArgumentException_WhenPermissionTypeNotFound()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionTypeRepo = new Mock<IPermissionTypeRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();
        
        mockPermissionTypeRepo.Setup(r => r.GetByidAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((PermissionType)null!);
        mockUnitOfWork.Setup(u => u.PermissionTypeRepository).Returns(mockPermissionTypeRepo.Object);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);
        
        var handler = new RequestPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);;
        var command = new RequestPermissionCommand("Jane", "Smith", 99, DateTime.Now);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
    }
} 