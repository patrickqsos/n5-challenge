using MediatR;
using N5Challenge.Dtos;

namespace N5Challenge.Commands;

public record RequestPermissionCommand(string EmployeeForename, string EmployeeSurname, int PermissionType, DateTime PermissionDate) : IRequest<PermissionDto>;