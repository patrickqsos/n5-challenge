using MediatR;
using N5Challenge.Constants;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Queries;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Handlers;

public class GetPermissionsQueryHandler(IPermissionRepository permissionRepository, IKafkaProducerService kafkaProducerService)
    : IRequestHandler<GetPermissionsQuery, IReadOnlyList<PermissionDto>>
{
    private readonly ILogger _logger = Log.ForContext<GetPermissionsQueryHandler>();

    public async Task<IReadOnlyList<PermissionDto>> Handle(GetPermissionsQuery query, CancellationToken ct)
    {
        _logger.Information("Querying all permissions");
        var rawList = await permissionRepository.GetAllAsync(ct);
        
        _logger.Information("Found: {permissionCount} permissions. Returning page: {pageNumber} with size: {pageSize}", rawList.Count, query.Page, query.PageSize);

        await kafkaProducerService.Send(new KafkaMessageDto(Guid.NewGuid(), OperationEnum.get));
        
        var list = rawList
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PermissionDto(p.Id, p.EmployeeForename, p.EmployeeSurname, p.PermissionTypeNavigation.Description, p.PermissionDate))
            .ToList();

        _logger
            .ForContext(SerilogConstants.LogType, SerilogConstants.LogRegistry)
            .ForContext(SerilogConstants.Operation, nameof(OperationEnum.get))
            //.ForContext(SerilogConstants.RegistryData, list, true)
            .Information("Logging registry data");
        
        return list;
    }
}