using MediatR;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Queries;
using N5Challenge.Repositories.Interfaces;

namespace N5Challenge.Handlers;

public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPermissionTypeRepository  _permissionTypeRepository;
    
    public GetPermissionsQueryHandler(IPermissionRepository permissionRepository, IPermissionTypeRepository permissionTypeRepository)
    {
        _permissionRepository     = permissionRepository;
        _permissionTypeRepository = permissionTypeRepository;
    }
    
    public async Task<IReadOnlyList<PermissionDto>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        IReadOnlyList<Permissions> list;

        list = await _permissionRepository.GetAllAsync(ct);
        
        return list
            .Skip((query.page - 1) * query.pageSize)
            .Take(query.pageSize)
            .Select(p => new PermissionDto(p.Id, p.EmployeeName, p.PermissionTypeId.ToString(), p.PermissionDate))
            .ToList();
    }
}