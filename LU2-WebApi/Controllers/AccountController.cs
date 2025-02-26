using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountInfoRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(IAccountRepository accountInfoRepository, IAuthenticationService authenticationService, UserManager<AppUser> userManager)
    {
        _accountInfoRepository = accountInfoRepository ?? throw new ArgumentNullException(nameof(accountInfoRepository));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    // POST: /account/register
    [HttpPost("register")]
    public async Task<ActionResult> Register(AccountRequest request)
    {
        try
        {
            var result = await _accountInfoRepository.RegisterUser(request);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during registration: {ex.Message}");
        }
    }

    // POST: /account/login
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        try
        {
            var result = await _accountInfoRepository.LoginUser(request);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during login: {ex.Message}");
        }
    }

    // POST: /account/logout
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var result = await _accountInfoRepository.LogoutUser();
            return Ok(result.SuccessMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during logout: {ex.Message}");
        }
    }

    // GET: /account/username
    [HttpGet("username")]
    [Authorize(Policy = "CanReadEntity")]
    public async Task<ActionResult<string>> GetUserName()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");
            var userName = await _accountInfoRepository.GetUserName(userId);
            if (string.IsNullOrEmpty(userName))
                return NotFound("User not found.");
            return Ok(userName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user name: {ex.Message}");
        }
    }

    // GET: /account/claims
    [HttpGet("claims")]
    [Authorize(Policy = "AccessEntity")]
    public async Task<ActionResult<IEnumerable<UserClaimDTO>>> GetClaims()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var claims = await _accountInfoRepository.GetUserClaims(userId);

            if (claims == null || !claims.Any())
                return NotFound("No claims found for the user.");

            var claimDTOs = claims.Select(claim => new UserClaimDTO
            {
                Id = claim.Id,
                UserId = claim.UserId.ToString(),
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            }).ToList();

            return Ok(claimDTOs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user claims: {ex.Message}");
        }
    }
}
