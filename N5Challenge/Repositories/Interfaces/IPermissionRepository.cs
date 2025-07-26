using N5Challenge.Domain;

namespace N5Challenge.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<Permission>> GetAllAsync(CancellationToken ct = default);

    Task<Permission> CreateAsync(Permission entity, CancellationToken ct = default);
    
    void Update(Permission entity);
}