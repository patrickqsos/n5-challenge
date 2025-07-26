using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Repositories;

public class PermissionRepository(N5DbContext ctx) : IPermissionRepository
{
    private readonly N5DbContext _ctx = ctx;
}