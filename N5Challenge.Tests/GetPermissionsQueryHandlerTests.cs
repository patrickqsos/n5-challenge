using Moq;
using N5Challenge.Handlers;
using N5Challenge.Queries;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services.Interfaces;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;

public class GetPermissionsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPagedPermissions_AndSendsKafkaMessage()
    {
        var mockPermissionRepo = new Mock<IPermissionRepository>();
        var mockKafkaService = new Mock<IKafkaProducerService>();
        var permissionType = new PermissionType { Id = 1, Description = "Test" };
        var permissions = new List<Permission>
        {
            new Permission { Id = 1, EmployeeForename = "John", EmployeeSurname = "Doe", PermissionTypeId = 1, PermissionType = permissionType, PermissionDate = DateTime.Now },
            new Permission { Id = 2, EmployeeForename = "Jane", EmployeeSurname = "Smith", PermissionTypeId = 1, PermissionType = permissionType, PermissionDate = DateTime.Now }
        };
        
        mockPermissionRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(permissions);
        mockKafkaService.Setup(k => k.Send(It.IsAny<KafkaMessageDto>(), It.IsAny<string>())).ReturnsAsync((Confluent.Kafka.DeliveryResult<Confluent.Kafka.Null, string>)null!);
        
        var handler = new GetPermissionsQueryHandler(mockPermissionRepo.Object, mockKafkaService.Object);
        var query = new GetPermissionsQuery(1, 1);
        
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("John", result.First().EmployeeForename);
        mockKafkaService.Verify(k => k.Send(It.Is<KafkaMessageDto>(m => m.OperationName == OperationEnum.get), It.IsAny<string>()), Times.Once);
    }
} 