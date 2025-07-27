using MediatR;
using Microsoft.AspNetCore.Mvc;
using N5Challenge.Commands;
using N5Challenge.Dtos;
using N5Challenge.Queries;

namespace N5Challenge.Controllers;

[ApiController]
[Route("permissions")]
public class PermissionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Retrieves a paginated list of permissions based on the specified parameters.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items to include per page. Defaults to 10.</param>
    /// <param name="ct">The cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains an IActionResult with a list of permissions as its content.</returns>
    [HttpGet("get")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetPermissionsQuery(page, pageSize);
        var result = await mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Requests a new permission based on the provided details.
    /// </summary>
    /// <param name="command">The command containing the details of the permission request, including employee information, permission type, and date.</param>
    /// <param name="ct">The cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains an IActionResult with the ID of the newly created permission as its content.</returns>
    [HttpPost("request")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> RequestPermission([FromBody] RequestPermissionCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetPermissions), new { result.Id }, result.Id);
    }

    /// <summary>
    /// Modifies an existing permission based on the provided parameters.
    /// </summary>
    /// <param name="id">The unique identifier of the permission to be modified, provided in the route.</param>
    /// <param name="command">The command containing the updated permission details.</param>
    /// <param name="ct">The cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The task result contains an IActionResult indicating the status of the operation.</returns>
    [HttpPut("modify/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ModifyPermission([FromRoute] int id,
        [FromBody] ModifyPermissionCommand command,
        CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match");

        await mediator.Send(command, ct);
        return NoContent();
    }
}
