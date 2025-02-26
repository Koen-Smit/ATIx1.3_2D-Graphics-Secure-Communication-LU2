using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("share")]
public class ShareController : ControllerBase
{
    private readonly IShareRepository _shareRepository;
    private readonly IAuthenticationService _authenticationService;

    public ShareController(IShareRepository shareRepository, IAuthenticationService authenticationService)
    {
        _shareRepository = shareRepository ?? throw new ArgumentNullException(nameof(shareRepository));
        _authenticationService = authenticationService;
    }

    private bool TryGetAuthenticatedUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
        return !string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out userId);
    }

    // POST: /share/scene
    [HttpPost("scene")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult> ShareSceneWithUser([FromBody] ShareRequest request)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            if (request == null || string.IsNullOrEmpty(request.SharedUserName) || request.WorldId == Guid.Empty)
                return BadRequest("Invalid input.");

            var result = await _shareRepository.ShareSceneWithUser(userId, request.SharedUserName, request.WorldId);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(new { message = result.SuccessMessage });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while sharing the world: {ex.Message}");
        }
    }

    // GET: /share/scenes
    [HttpGet("scenes")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult> GetAllSharedEnvironments()
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var sharedWorlds = await _shareRepository.GetAllSharedEnvironments(userId);

            if (sharedWorlds == null)
            {
                return NotFound("No worlds available for sharing.");
            }

            return Ok(sharedWorlds);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching shared worlds: {ex.Message}");
        }
    }






}
