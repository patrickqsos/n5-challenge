using Microsoft.EntityFrameworkCore;
using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Repositories;

public class PermissionRepository(N5DbContext ctx) : IPermissionRepository
{
    private readonly N5DbContext _ctx = ctx;

    public async Task<List<Permissions>> GetAllAsync(CancellationToken ct = default)
    {
        return await _ctx.Permissions.ToListAsync(ct);
    }
}