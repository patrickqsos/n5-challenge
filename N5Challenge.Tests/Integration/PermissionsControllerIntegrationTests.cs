using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using N5Challenge.Commands;
using N5Challenge.Domain;
using N5Challenge.Dtos;

namespace N5Challenge.Tests.Integration;

public class PermissionsControllerIntegrationTests : TestBase
{
    private HttpClient _client = null!;
    private N5DbContext _context = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _client = Factory.CreateClient();
        var scope = Factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<N5DbContext>();
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnOkWithPermissions()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/permissions/get?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var permissions = await response.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        permissions.Should().NotBeNull();
        permissions!.Should().HaveCount(2);
        permissions.Should().Contain(p => p.EmployeeForename == "Patricio");
        permissions.Should().Contain(p => p.EmployeeForename == "Nassia");
    }

    [Fact]
    public async Task GetPermissions_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/permissions/get?page=1&pageSize=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var permissions = await response.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        permissions.Should().NotBeNull();
        permissions!.Should().HaveCount(1);
    }

    [Fact]
    public async Task RequestPermission_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        await SeedDatabaseAsync();
        var vacationType = _context.PermissionType.First(pt => pt.Description == "Vacation");
        var command = new RequestPermissionCommand("Paolo", "Quiroz", vacationType.Id, DateTime.Now);

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/permissions/request", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var permissionId = await response.Content.ReadAsStringAsync();
        permissionId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RequestPermission_WithInvalidPermissionType_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();
        var maxId = _context.PermissionType.Max(pt => pt.Id);
        var command = new RequestPermissionCommand("Paolo", "Quiroz", maxId + 1000, DateTime.Now);

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/permissions/request", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ModifyPermission_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        await SeedDatabaseAsync();
        var permission = _context.Permission.First(p => p.EmployeeForename == "Patricio");
        var vacationType = _context.PermissionType.First(pt => pt.Description == "Vacation");
        var command = new ModifyPermissionCommand(permission.Id, "Patricio Updated", "Quispe Updated", vacationType.Id, DateTime.Now.AddDays(1));

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/permissions/modify/{permission.Id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync("/permissions/get");
        var permissions = await getResponse.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        var updatedPermission = permissions!.FirstOrDefault(p => p.Id == permission.Id);
        updatedPermission.Should().NotBeNull();
        updatedPermission!.EmployeeForename.Should().Be("Patricio Updated");
        updatedPermission!.EmployeeSurname.Should().Be("Quispe Updated");
    }

    [Fact]
    public async Task ModifyPermission_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();
        var permission = _context.Permission.First(p => p.EmployeeForename == "Patricio");
        var vacationType = _context.PermissionType.First(pt => pt.Description == "Vacation");
        var command = new ModifyPermissionCommand(permission.Id, "Patricio Updated", "Quispe Updated", vacationType.Id, DateTime.Now.AddDays(1));

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/permissions/modify/{permission.Id + 1}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ModifyPermission_WithNonExistentPermission_ShouldReturnBadRequest()
    {
        // Arrange
        await SeedDatabaseAsync();
        var maxId = _context.Permission.Max(p => p.Id);
        var vacationType = _context.PermissionType.First(pt => pt.Description == "Vacation");
        var command = new ModifyPermissionCommand(maxId + 1000, "Non Existent", "User", vacationType.Id, DateTime.Now);

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/permissions/modify/{maxId + 1000}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EndToEnd_CompleteWorkflow_ShouldWorkCorrectly()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Step 1: Get initial permissions
        var initialResponse = await _client.GetAsync("/permissions/get");
        initialResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var initialPermissions = await initialResponse.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        var initialCount = initialPermissions!.Count();

        // Step 2: Request a new permission
        var personalType = _context.PermissionType.First(pt => pt.Description == "Personal");
        var requestCommand = new RequestPermissionCommand("Paolo", "Quiroz", personalType.Id, DateTime.Now);

        var requestJson = JsonSerializer.Serialize(requestCommand);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var requestResponse = await _client.PostAsync("/permissions/request", requestContent);
        requestResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Step 3: Verify the permission was added
        var afterRequestResponse = await _client.GetAsync("/permissions/get");
        var afterRequestPermissions = await afterRequestResponse.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        afterRequestPermissions!.Count().Should().Be(initialCount + 1);
        afterRequestPermissions.Should().Contain(p => p.EmployeeForename == "Paolo");

        // Step 4: Modify the new permission
        var newPermission = afterRequestPermissions.First(p => p.EmployeeForename == "Paolo");
        var modifyCommand = new ModifyPermissionCommand(newPermission.Id, "Paolo Updated", "Quiroz Updated", personalType.Id, DateTime.Now.AddDays(5));

        var modifyJson = JsonSerializer.Serialize(modifyCommand);
        var modifyContent = new StringContent(modifyJson, Encoding.UTF8, "application/json");
        var modifyResponse = await _client.PutAsync($"/permissions/modify/{newPermission.Id}", modifyContent);
        modifyResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Step 5: Verify the modification
        var finalResponse = await _client.GetAsync("/permissions/get");
        var finalPermissions = await finalResponse.Content.ReadFromJsonAsync<IEnumerable<PermissionDto>>();
        var modifiedPermission = finalPermissions!.First(p => p.Id == newPermission.Id);
        modifiedPermission.EmployeeForename.Should().Be("Paolo Updated");
        modifiedPermission.EmployeeSurname.Should().Be("Quiroz Updated");
    }
} 