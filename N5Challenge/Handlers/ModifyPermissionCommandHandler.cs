using MediatR;
using N5Challenge.Commands;
using N5Challenge.Constants;
using N5Challenge.Domain;
using N5Challenge.Dtos;
using N5Challenge.Enums;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services.Interfaces;
using Serilog;
using ILogger = Serilog.ILogger;

namespace N5Challenge.Handlers;

public class ModifyPermissionCommandHandler(IUnitOfWork unitOfWork, IKafkaProducerService kafkaProducerService) : IRequestHandler<ModifyPermissionCommand>
{
    private readonly ILogger _logger = Log.ForContext<ModifyPermissionCommandHandler>();

    public async Task Handle(ModifyPermissionCommand command, CancellationToken ct)
    {
        _logger.Information("Validating permission id: {permissionId}", command.Id);

        var permission = await unitOfWork.PermissionRepository.GetByIdAsync(command.Id, ct);
        if(permission is null)
            throw new ArgumentException("Permission not found");

        _logger.Information("Permission id is valid");

        _logger.Information("Validating permission type id: {permissionTypeId}", command.PermissionTypeId);

        var permissionType = await unitOfWork.PermissionTypeRepository.GetByidAsync(command.PermissionTypeId, ct);
        if(permissionType is null)
            throw new ArgumentException("Permission type id not found");

        _logger.Information("Permission type id is valid");

        permission.EmployeeForename = command.EmployeeForename;
        permission.EmployeeSurname = command.EmployeeSurname;
        permission.PermissionTypeId = command.PermissionTypeId;
        permission.PermissionDate = command.PermissionDate;
        
        unitOfWork.PermissionRepository.Update(permission);
        await unitOfWork.SaveChangesAsync(ct);
        
        await kafkaProducerService.Send(new KafkaMessageDto(Guid.NewGuid(), OperationEnum.modify));
        
        _logger
            .ForContext(SerilogConstants.LogType, SerilogConstants.LogRegistry)
            .ForContext(SerilogConstants.Operation, nameof(OperationEnum.modify))
            .ForContext(SerilogConstants.RegistryData, permission, true)
            .Information("Logging registry data");
    }
}