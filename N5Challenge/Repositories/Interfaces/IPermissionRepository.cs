using N5Challenge.Domain;

namespace N5Challenge.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<List<Permissions>> GetAllAsync(CancellationToken ct = default);
    
}