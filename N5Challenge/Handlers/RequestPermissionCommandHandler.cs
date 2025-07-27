using MediatR;
using N5Challenge.Commands;
using N5Challenge.Constants;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Exceptions;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Handlers;

public class RequestPermissionCommandHandler(IUnitOfWork unitOfWork, IKafkaProducerService kafkaProducerService) : IRequestHandler<RequestPermissionCommand, PermissionDto>
{
    private readonly ILogger _logger = Log.ForContext<RequestPermissionCommandHandler>();

    public async Task<PermissionDto> Handle(RequestPermissionCommand command, CancellationToken ct)
    {
        _logger.Information("Validating permission type id: {permissionTypeId}", command.PermissionTypeId);

        var permissionType = await unitOfWork.PermissionTypeRepository.GetByidAsync(command.PermissionTypeId, ct);
        if(permissionType is null)
            throw new NotFoundException("Permission type id not found");

        _logger.Information("Permission type id is valid");

        var entity = new Permission
        {
            EmployeeForename = command.EmployeeForename,
            EmployeeSurname = command.EmployeeSurname,
            PermissionTypeId = command.PermissionTypeId,
            PermissionDate = command.PermissionDate
        };
        var entityCreated = await unitOfWork.PermissionRepository.CreateAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        await kafkaProducerService.Send(new KafkaMessageDto(Guid.NewGuid(), OperationEnum.request));

        _logger
            .ForContext(SerilogConstants.LogType, SerilogConstants.LogRegistry)
            .ForContext(SerilogConstants.Operation, nameof(OperationEnum.request))
            .ForContext(SerilogConstants.RegistryData, entity, true)
            .Information("Logging registry data");
        
        //todo: use automapper here
        return new PermissionDto(entityCreated.Id, entityCreated.EmployeeForename, entityCreated.EmployeeSurname, entityCreated.PermissionType.Description, entityCreated.PermissionDate) ;
    }
}