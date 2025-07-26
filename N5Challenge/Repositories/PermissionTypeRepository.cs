using N5Challenge.Domain;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Repositories;

public class PermissionTypeRepository(N5DbContext ctx) : IPermissionTypeRepository
{
    private readonly N5DbContext _ctx = ctx;
}