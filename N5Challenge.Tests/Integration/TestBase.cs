using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using N5Challenge.Domain;
using N5Challenge.Services.Interfaces;
using Moq;

namespace N5Challenge.Tests.Integration;

public abstract class TestBase : IAsyncLifetime
{
    protected WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public virtual async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>();
    }

    public async Task DisposeAsync()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }

    protected async Task SeedDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<N5DbContext>();

        context.Permission.RemoveRange(context.Permission);
        context.PermissionType.RemoveRange(context.PermissionType);
        await context.SaveChangesAsync();

        var permissionTypes = new List<PermissionType>
        {
            new PermissionType { Description = "Vacation" },
            new PermissionType { Description = "Sick Leave" },
            new PermissionType { Description = "Personal" }
        };
        context.PermissionType.AddRange(permissionTypes);
        await context.SaveChangesAsync();

        var vacationType = context.PermissionType.First(pt => pt.Description == "Vacation");
        var sickLeaveType = context.PermissionType.First(pt => pt.Description == "Sick Leave");
        var personalType = context.PermissionType.First(pt => pt.Description == "Personal");

        var permissions = new List<Permission>
        {
            new Permission
            {
                EmployeeForename = "Patricio",
                EmployeeSurname = "Quispe",
                PermissionType = vacationType.Id,
                PermissionDate = DateTime.Now.AddDays(-5)
            },
            new Permission
            {
                EmployeeForename = "Nassia",
                EmployeeSurname = "Salvador",
                PermissionType = sickLeaveType.Id,
                PermissionDate = DateTime.Now.AddDays(-3)
            }
        };

        context.Permission.AddRange(permissions);
        await context.SaveChangesAsync();
    }
} 