using Microsoft.EntityFrameworkCore;
using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Repositories;

public class PermissionRepository(N5DbContext ctx) : IPermissionRepository
{
    private readonly ILogger _logger = Log.ForContext<PermissionRepository>();

    /// <summary>
    /// Retrieves a permission by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the permission to be retrieved.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the permission with the specified identifier, or null if it does not exist.</returns>
    public async Task<Permission?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await ctx.Permission.Include(p => p.PermissionTypeNavigation).FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    /// <summary>
    /// Retrieves all permissions asynchronously.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all permissions.</returns>
    public async Task<List<Permission>> GetAllAsync(CancellationToken ct = default)
    {
        return await ctx.Permission.Include(p => p.PermissionTypeNavigation).ToListAsync(ct);
    }

    /// <summary>
    /// Creates a new permission asynchronously.
    /// </summary>
    /// <param name="entity">The permission entity to be created.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created permission entity.</returns>
    public async Task<Permission> CreateAsync(Permission entity, CancellationToken ct = default)
    {
        var entityInserted = await ctx.Permission.AddAsync(entity, ct);
        var pk = entityInserted.Metadata.FindPrimaryKey()?.Properties.Select(p => entityInserted.Property(p.Name).CurrentValue).Single();
        _logger.Information("Entity {entityName} inserted with id {entityPk}", typeof(Permission).Name, pk);

        return entityInserted.Entity;
    }

    /// <summary>
    /// Updates an existing permission entity in the database.
    /// </summary>
    /// <param name="entity">The permission entity to be updated.</param>
    public void Update(Permission entity)
    {
        _logger.Information("Updating entity with id: {id}", entity.Id);
        ctx.Permission.Update(entity);
    }
}