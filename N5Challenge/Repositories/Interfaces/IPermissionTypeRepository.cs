using N5Challenge.Domain;

namespace N5Challenge.Repositories.Interfaces;

public interface IPermissionTypeRepository
{
    Task<List<PermissionType>> GetAllAsync(CancellationToken ct = default);
    Task<PermissionType?> GetByidAsync(int id, CancellationToken ct = default);
}