using MediatR;
using Microsoft.AspNetCore.Mvc;
using N5Challenge.Dtos;
using N5Challenge.Queries;

namespace N5Challenge.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ILogger<PermissionsController> _logger;

    private readonly IMediator _mediator;
    
    public PermissionsController(ILogger<PermissionsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = new GetPermissionsQuery(page, pageSize);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
    
}
