using Moq;
using N5Challenge.Handlers;
using N5Challenge.Commands;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Exceptions;
using N5Challenge.Services.Interfaces;

namespace N5Challenge.Tests.Unit;

public class RequestPermissionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPermissionDto_WhenPermissionTypeExists()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionTypeRepo = new Mock<IPermissionTypeRepository>();
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();

        var permissionType = new PermissionType { Id = 1, Description = "Test" };
        var permission = new Permission { Id = 1, EmployeeForename = "Patricio", EmployeeSurname = "Quispe", PermissionType = 1, PermissionTypeNavigation = permissionType, PermissionDate = System.DateTime.Now };
        
        mockPermissionTypeRepo.Setup(r => r.GetByidAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(permissionType);
        mockPermissionRepo.Setup(r => r.CreateAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>())).ReturnsAsync(permission);
        mockUnitOfWork.Setup(u => u.PermissionTypeRepository).Returns(mockPermissionTypeRepo.Object);
        mockUnitOfWork.Setup(u => u.PermissionRepository).Returns(mockPermissionRepo.Object);
        mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);

        var handler = new RequestPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);
        var command = new RequestPermissionCommand("Patricio", "Quispe", 1, permission.PermissionDate);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Patricio", result.EmployeeForename);
        Assert.Equal("Quispe", result.EmployeeSurname);
        Assert.Equal("Test", result.PermissionTypeDesc);
        mockKafkaService.Verify(k => k.Send(It.Is<KafkaMessageDto>(m => m.OperationName == OperationEnum.request), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsArgumentException_WhenPermissionTypeNotFound()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockPermissionTypeRepo = new Mock<IPermissionTypeRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();
        
        mockPermissionTypeRepo.Setup(r => r.GetByidAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((PermissionType)null!);
        mockUnitOfWork.Setup(u => u.PermissionTypeRepository).Returns(mockPermissionTypeRepo.Object);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);
        
        var handler = new RequestPermissionCommandHandler(mockUnitOfWork.Object, mockKafkaService.Object);;
        var command = new RequestPermissionCommand("Patricio", "Quispe", 99, DateTime.Now);

        // Act
        // Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
} 