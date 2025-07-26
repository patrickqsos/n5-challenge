using N5Challenge.Domain;

namespace N5Challenge.Repositories.Interfaces;

public interface IPermissionTypeRepository
{
    Task<PermissionType?> GetByidAsync(int id, CancellationToken ct = default);
}