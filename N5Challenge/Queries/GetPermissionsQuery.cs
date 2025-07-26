using MediatR;
using N5Challenge.Domain;
using N5Challenge.Dtos;

namespace N5Challenge.Queries;

public record GetPermissionsQuery(int Page, int PageSize) : IRequest<IReadOnlyList<PermissionDto>>;