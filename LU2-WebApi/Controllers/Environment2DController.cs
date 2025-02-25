using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
[ApiController]
[Route("environment2d")]
public class Environment2DController : ControllerBase
{
    private readonly IEnvironment2DRepository _environmentRepository;
    private readonly IAuthenticationService _authenticationService;

    public Environment2DController(IEnvironment2DRepository environmentRepository, IAuthenticationService authenticationService)
    {
        _environmentRepository = environmentRepository;
        _authenticationService = authenticationService;
    }

    // GET: /environment2d
    [HttpGet]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<Environment2DDto>>> GetEnvironments()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");

            var environments = await _environmentRepository.GetUserEnvironmentsAsync(userId);
            var environmentDtos = environments.Select(e => new Environment2DDto
            {
                Id = e.Id,
                Name = e.Name,
                MaxLength = e.MaxLength,
                MaxHeight = e.MaxHeight,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                EnvironmentType = e.EnvironmentType
            });

            return Ok(environmentDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user environments: {ex.Message}");
        }
    }

    // GET: /environment2d/{environmentId}
    [HttpGet("{environmentId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<Environment2DDto>> GetEnvironmentById(Guid environmentId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");

            var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var environmentDto = new Environment2DDto
            {
                Id = environment.Id,
                Name = environment.Name,
                MaxLength = environment.MaxLength,
                MaxHeight = environment.MaxHeight,
                CreatedAt = environment.CreatedAt,
                UpdatedAt = environment.UpdatedAt,
                EnvironmentType = environment.EnvironmentType
            };

            return Ok(environmentDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the environment: {ex.Message}");
        }
    }

    // POST: /environment2d
    [HttpPost]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> CreateEnvironment([FromBody] CreateEnvironmentRequest request)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");

            var environmentCount = await _environmentRepository.GetEnvironmentCountByUserIdAsync(userId);
            if (environmentCount >= 5)
                return BadRequest("You cannot create more than 5 environments.");

            var existingEnvironment = await _environmentRepository.GetByNameAsync(userId, request.Name);
            if (existingEnvironment != null)
                return Conflict("An environment with this name already exists.");

            var newEnvironment = new Environment2D
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                MaxLength = 100,
                MaxHeight = 50,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                EnvironmentType = request.EnvironmentType
            };

            var success = await _environmentRepository.CreateEnvironmentAsync(newEnvironment);
            if (!success)
                return StatusCode(500, "An error occurred while creating the environment.");

            return CreatedAtAction(nameof(GetEnvironments), new { id = newEnvironment.Id }, newEnvironment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user environments: {ex.Message}");
        }
    }

    // DELETE: /environment2d/{environmentId}
    [HttpDelete("{environmentId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentId)
    {
        try
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
                return Unauthorized("User is not authenticated.");

            var result = await _environmentRepository.DeleteEnvironmentAsync(environmentId, parsedUserId);

            if (result)
                return Ok("Environment deleted successfully.");
            else
                return NotFound("Environment not found or does not belong to the user.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user environments: {ex.Message}");
        }
    }

    // GET: /object2d/{environment2dId}/all
    [HttpGet("{environmentId}/allObjects")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<Object2D>>> GetAllObjects(Guid environmentId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");

            var environment = await _environmentRepository.GetEnvironmentByIdAsync(environmentId, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var objects = await _environmentRepository.GetObjectsByEnvironment2DIdAsync(environmentId);

            if (objects == null || !objects.Any())
                return NotFound("No objects found for this environment.");

            return Ok(objects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching objects: {ex.Message}");
        }
    }

    //GET: /object2d/{objectId}
    [HttpGet("GetObject/{objectId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<Object2D>> GetObjectById(Guid objectId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");
            var object2D = await _environmentRepository.GetObjectByIdAsync(objectId);
            if (object2D == null)
                return NotFound("Object not found.");
            return Ok(object2D);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the object: {ex.Message}");
        }
    }

    // POST: /object2d/createObject
    [HttpPost("createObject")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<Object2D>> CreateObject([FromBody] CreateObject2DRequest createRequest)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");

            var environment = await _environmentRepository.GetEnvironmentByIdAsync(createRequest.Environment2D_Id, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var newObject2D = new Object2D
            {
                Id = Guid.NewGuid(),
                PrefabId = createRequest.PrefabId,
                PositionX = createRequest.PositionX,
                PositionY = createRequest.PositionY,
                ScaleX = createRequest.ScaleX,
                ScaleY = createRequest.ScaleY,
                RotationZ = createRequest.RotationZ,
                SortingLayer = createRequest.SortingLayer,
                Environment2D_Id = createRequest.Environment2D_Id
            };

            var result = await _environmentRepository.CreateObject2DAsync(newObject2D);

            if (result == 0)
                return BadRequest("Failed to create Object2D.");

            return CreatedAtAction(nameof(GetAllObjects), new { environmentId = newObject2D.Environment2D_Id }, newObject2D);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the object: {ex.Message}");
        }
    }

    // DELETE: /object2d/{objectId}
    [HttpDelete("deleteObject/{objectId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> DeleteObject(Guid objectId)
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated.");
            var object2D = await _environmentRepository.GetObjectByIdAsync(objectId);
            if (object2D == null)
                return NotFound("Object not found.");
            var result = await _environmentRepository.DeleteObjectAsync(objectId);
            if (result)
                return Ok("Object deleted successfully.");
            else
                return NotFound("Object not found or does not belong to the user.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the object: {ex.Message}");
        }
    }
}
