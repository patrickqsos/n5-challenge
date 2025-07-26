using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Repositories;

public class UnitOfWork(N5DbContext context, IPermissionRepository permissionRepository, IPermissionTypeRepository permissionTypeRepository)
    : IUnitOfWork
{
    public IPermissionRepository PermissionRepository { get; } = permissionRepository;
    public IPermissionTypeRepository PermissionTypeRepository { get; } = permissionTypeRepository;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}