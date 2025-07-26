namespace N5Challenge.Repositories.Interfaces;

public interface IUnitOfWork
{
    IPermissionRepository PermissionRepository { get; }
    IPermissionTypeRepository PermissionTypeRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}