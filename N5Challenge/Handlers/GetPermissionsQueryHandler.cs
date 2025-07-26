using MediatR;
using N5Challenge.Dtos;
using N5Challenge.Queries;
using N5Challenge.Repositories.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Handlers;

public class GetPermissionsQueryHandler(IPermissionRepository permissionRepository, IPermissionTypeRepository permissionTypeRepository)
    : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    private readonly ILogger _logger = Log.ForContext<GetPermissionsQueryHandler>();

    public async Task<IReadOnlyList<PermissionDto>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        var rawList = await permissionRepository.GetAllAsync(ct);
        
        _logger.Information("Found: {permissionCount} permissions. Returning page: {pageNumber} with size: {pageSize}", rawList.Count, query.page, query.pageSize);
        
        return rawList
            .Skip((query.page - 1) * query.pageSize)
            .Take(query.pageSize)
            .Select(p => new PermissionDto(p.Id, p.EmployeeName, p.PermissionTypes.Description, p.PermissionDate))
            .ToList();
    }
}