using MediatR;

namespace N5Challenge.Commands;

public record ModifyPermissionCommand(int Id, string EmployeeForename, string EmployeeSurname, int PermissionType, DateTime PermissionDate) : IRequest;