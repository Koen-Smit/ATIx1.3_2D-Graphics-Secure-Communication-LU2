using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("scene")]
public class SceneController : ControllerBase
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IEntityRepository _entityRepository;
    private readonly IAuthenticationService _authenticationService;
    private bool TryGetAuthenticatedUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
        return !string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out userId);
    }

    public SceneController(IEnvironmentRepository environmentRepository, IEntityRepository entityRepository, IAuthenticationService authenticationService)
    {
        _environmentRepository = environmentRepository;
        _entityRepository = entityRepository;
        _authenticationService = authenticationService;
    }

    // GET: /scene
    [HttpGet]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<EnvironmentDTO>>> GetEnvironments()
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var environments = await _environmentRepository.GetEnvironmentsFromUser(userId);
            var environmentDtos = environments.Select(e => new EnvironmentDTO
            {
                Id = e!.Id,
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

    // GET: /scene/{environmentId}
    [HttpGet("{environmentId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<EnvironmentDTO>> GetEnvironmentById(Guid environmentId)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");


            var environment = await _environmentRepository.GetEnvironmentById(environmentId, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var environmentDTO = new EnvironmentDTO
            {
                Id = environment.Id,
                Name = environment.Name,
                MaxLength = environment.MaxLength,
                MaxHeight = environment.MaxHeight,
                CreatedAt = environment.CreatedAt,
                UpdatedAt = environment.UpdatedAt,
                EnvironmentType = environment.EnvironmentType
            };

            return Ok(environmentDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the environment: {ex.Message}");
        }
    }

    // POST: /scene
    [HttpPost]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> CreateEnvironment([FromBody] EnvironmentRequest request)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var newEnvironment = new Environment
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                MaxLength = request.MaxLength,
                MaxHeight = request.MaxHeight,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                EnvironmentType = request.EnvironmentType
            };

            var result = await _environmentRepository.CreateEnvironment(newEnvironment);

            if (result!.IsSuccess)
                return CreatedAtAction(nameof(GetEnvironments), new { id = newEnvironment.Id }, newEnvironment);

            return result.ErrorMessage switch
            {
                "User cannot have more than 5 environments." => BadRequest(result.ErrorMessage),
                "An environment with this name already exists." => Conflict(result.ErrorMessage),
                _ => StatusCode(500, result.ErrorMessage)
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the environment: {ex.Message}");
        }
    }


    // DELETE: /scene/{environmentId}
    [HttpDelete("{environmentId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentId)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");
            var result = await _environmentRepository.DeleteEnvironment(environmentId, userId);

            if (result!.IsSuccess)
                return Ok("Environment deleted successfully.");

            return result.ErrorMessage switch
            {
                "Environment not found or does not belong to the user." => NotFound(result.ErrorMessage),
                _ => StatusCode(500, result.ErrorMessage)
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the environment: {ex.Message}");
        }
    }


    // GET: /scene/{environmentId}/entities
    [HttpGet("{environmentId}/entities")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<IEnumerable<Entity>>> GetAllEntities(Guid environmentId)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");


            var environment = await _environmentRepository.GetEnvironmentById(environmentId, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var entities = await _entityRepository.GetEntitiesFromEnvironment(environmentId);

            if (entities == null || !entities.Any())
                return NotFound("No entities found for this environment.");

            return Ok(entities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching entities: {ex.Message}");
        }
    }

    //GET: /scene/entity/{entityId}
    [HttpGet("entity/{entityId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<Entity>> GetEntityById(Guid entityId)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var entity = await _entityRepository.GetEntityById(entityId);
            if (entity == null)
                return NotFound("Entity not found.");
            return Ok(entity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching the entities: {ex.Message}");
        }
    }

    // POST: /scene/entity
    [HttpPost("entity")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<EntityDTO>> CreateEntity([FromBody] EntityRequest createRequest)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var environment = await _environmentRepository.GetEnvironmentById(createRequest.EnvironmentId, userId);
            if (environment == null)
                return NotFound("Environment not found or does not belong to the user.");

            var newEntity = new Entity
            {
                Id = Guid.NewGuid(),
                Prefab_Id = createRequest.Prefab_Id,
                PositionX = createRequest.PositionX,
                PositionY = createRequest.PositionY,
                ScaleX = createRequest.ScaleX,
                ScaleY = createRequest.ScaleY,
                RotationZ = createRequest.RotationZ,
                SortingLayer = createRequest.SortingLayer,
                EnvironmentId = createRequest.EnvironmentId,

            };

            var result = await _entityRepository.CreateEntity(newEntity);

            if (result == 0)
                return BadRequest("Failed to create Entity.");

            return CreatedAtAction(nameof(GetAllEntities), new { environmentId = newEntity.EnvironmentId }, newEntity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the entity: {ex.Message}");
        }
    }

    // PUT: /scene/entity/{entityId}
    [HttpPut("entity/{entityId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> UpdateEntity(Guid entityId, [FromBody] EntityUpdateRequest updateRequest)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var entity = await _entityRepository.GetEntityById(entityId);
            if (entity == null)
                return NotFound("Entity not found.");

            entity.PositionX = updateRequest.PositionX;
            entity.PositionY = updateRequest.PositionY;
            entity.ScaleX = updateRequest.ScaleX;
            entity.ScaleY = updateRequest.ScaleY;
            entity.RotationZ = updateRequest.RotationZ;
            entity.SortingLayer = updateRequest.SortingLayer;
            entity.UpdatedAt = DateTime.UtcNow;

            var result = await _entityRepository.UpdateEntity(entity);

            if (result > 0)
                return Ok("Entity updated successfully.");

            return BadRequest("Failed to update entity.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the entity: {ex.Message}");
        }
    }

    // DELETE: /scene/entity/{entityId}
    [HttpDelete("entity/{entityId}")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<IActionResult> DeleteEntity(Guid entityId)
    {
        try
        {
            if (!TryGetAuthenticatedUserId(out var userId))
                return Unauthorized("User is not authenticated.");

            var entity = await _entityRepository.GetEntityById(entityId);
            if (entity == null)
                return NotFound("Entity not found.");
            var result = await _entityRepository.DeleteEntity(entityId);
            if (result)
                return Ok("Entity deleted successfully.");
            else
                return NotFound("Entity not found or does not belong to the user.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the entity: {ex.Message}");
        }
    }
}
