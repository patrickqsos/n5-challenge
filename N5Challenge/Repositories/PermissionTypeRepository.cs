using Microsoft.EntityFrameworkCore;
using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Repositories;

public class PermissionTypeRepository(N5DbContext ctx) : IPermissionTypeRepository
{
    /// <summary>
    /// Retrieves a list of all permission type entities asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a list of all permission type entities.</returns>
    public async Task<List<PermissionType>> GetAllAsync(CancellationToken ct = default)
    {
        return await ctx.PermissionType.ToListAsync(ct);
    }
    
    /// <summary>
    /// Retrieves a permission type entity by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the permission type to be retrieved.</param>
    /// <param name="ct">The cancellation token used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is the permission type entity matching the given identifier, or null if not found.</returns>
    public Task<PermissionType?> GetByidAsync(int id, CancellationToken ct = default)
    {
        return ctx.PermissionType.FirstOrDefaultAsync(pt => pt.Id == id, ct);
    }
}