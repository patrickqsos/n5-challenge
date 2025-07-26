using MediatR;
using N5Challenge.Domain;
using N5Challenge.Dtos;

namespace N5Challenge.Queries;

public record GetPermissionsQuery(int page, int pageSize) : IRequest<IReadOnlyList<PermissionDto>>;