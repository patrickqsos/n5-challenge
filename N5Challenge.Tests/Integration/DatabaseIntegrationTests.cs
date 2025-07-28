using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Tests.Integration;

public class DatabaseIntegrationTests : TestBase
{
    private N5DbContext _context = null!;
    private IPermissionRepository _permissionRepository = null!;
    private IPermissionTypeRepository _permissionTypeRepository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var scope = Factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<N5DbContext>();
        _permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();
        _permissionTypeRepository = scope.ServiceProvider.GetRequiredService<IPermissionTypeRepository>();
    }

    //[Fact]
    public async Task Database_ShouldBeCreatedSuccessfully()
    {
        // Act & Assert
        var canConnect = await _context.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }

    //[Fact]
    public async Task PermissionTypes_ShouldBeSeededCorrectly()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var permissionTypes = await _permissionTypeRepository.GetAllAsync();

        // Assert
        permissionTypes.Should().NotBeNull();
        permissionTypes.Should().HaveCount(3);
        permissionTypes.Should().Contain(pt => pt.Description == "Vacation");
        permissionTypes.Should().Contain(pt => pt.Description == "Sick Leave");
        permissionTypes.Should().Contain(pt => pt.Description == "Personal");
    }

    //[Fact]
    public async Task Permissions_ShouldBeSeededCorrectly()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var permissions = await _permissionRepository.GetAllAsync();

        // Assert
        permissions.Should().NotBeNull();
        permissions.Should().HaveCount(2);
        permissions.Should().Contain(p => p.EmployeeForename == "Patricio");
        permissions.Should().Contain(p => p.EmployeeForename == "Nassia");
    }

    //[Fact]
    public async Task CreatePermission_ShouldPersistToDatabase()
    {
        // Arrange
        await SeedDatabaseAsync();
        var vacationType = _context.PermissionType.First(pt => pt.Description == "Vacation");
        var newPermission = new Permission
        {
            EmployeeForename = "Paolo",
            EmployeeSurname = "Quiroz",
            PermissionType = vacationType.Id,
            PermissionDate = DateTime.Now
        };

        // Act
        await _permissionRepository.CreateAsync(newPermission);
        await _context.SaveChangesAsync();

        // Assert
        var retrievedPermission = await _permissionRepository.GetByIdAsync(newPermission.Id);
        retrievedPermission.Should().NotBeNull();
        retrievedPermission!.EmployeeForename.Should().Be("Paolo");
        retrievedPermission.EmployeeSurname.Should().Be("Quiroz");
        retrievedPermission.PermissionType.Should().Be(vacationType.Id);
    }

    //[Fact]
    public async Task UpdatePermission_ShouldPersistChangesToDatabase()
    {
        // Arrange
        await SeedDatabaseAsync();
        var permission = _context.Permission.First(p => p.EmployeeForename == "Patricio");
        var sickLeaveType = _context.PermissionType.First(pt => pt.Description == "Sick Leave");
        permission.Should().NotBeNull();

        // Act
        permission!.EmployeeForename = "Patricio Updated";
        permission.EmployeeSurname = "Quispe Updated";
        permission.PermissionType = sickLeaveType.Id;
        permission.PermissionDate = DateTime.Now.AddDays(1);

        _permissionRepository.Update(permission);
        await _context.SaveChangesAsync();

        // Assert
        var updatedPermission = await _permissionRepository.GetByIdAsync(permission.Id);
        updatedPermission.Should().NotBeNull();
        updatedPermission!.EmployeeForename.Should().Be("Patricio Updated");
        updatedPermission.EmployeeSurname.Should().Be("Quispe Updated");
        updatedPermission.PermissionType.Should().Be(sickLeaveType.Id);
    }
    
    //[Fact]
    public async Task GetPermissionById_WithValidId_ShouldReturnPermission()
    {
        // Arrange
        await SeedDatabaseAsync();
        var permission = _context.Permission.First(p => p.EmployeeForename == "Patricio");

        // Act
        var result = await _permissionRepository.GetByIdAsync(permission.Id);

        // Assert
        result.Should().NotBeNull();
        result!.EmployeeForename.Should().Be("Patricio");
        result.EmployeeSurname.Should().Be("Quispe");
        result.PermissionType.Should().Be(permission.PermissionType);
    }

    //[Fact]
    public async Task GetPermissionById_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var maxId = _context.Permission.Max(p => p.Id);

        // Act
        var permission = await _permissionRepository.GetByIdAsync(maxId + 1000);

        // Assert
        permission.Should().BeNull();
    }

    //[Fact]
    public async Task GetAllPermissions_ShouldReturnAllPermissions()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var permissions = await _permissionRepository.GetAllAsync();

        // Assert
        permissions.Should().NotBeNull();
        permissions.Should().HaveCount(2);
        permissions.Should().Contain(p => p.EmployeeForename == "Patricio");
        permissions.Should().Contain(p => p.EmployeeForename == "Nassia");
    }

    //[Fact]
    public async Task Permission_WithPermissionType_ShouldLoadNavigationProperty()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var permissions = await _context.Permission.Include(p => p.PermissionTypeNavigation).ToListAsync();

        // Assert
        permissions.Should().NotBeEmpty();
        permissions.Should().OnlyContain(p => p.PermissionTypeNavigation != null);
        permissions.Should().Contain(p => p.PermissionTypeNavigation!.Description == "Vacation");
        permissions.Should().Contain(p => p.PermissionTypeNavigation!.Description == "Sick Leave");
    }
} 