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
            if (result == null || !result.IsSuccess)
                return BadRequest(result?.ErrorMessage ?? "An unknown error occurred.");

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
            var loginResponse = await _accountInfoRepository.LoginUser(request);

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                return BadRequest(loginResponse?.Message ?? "An unknown error occurred.");

            return Ok(new
            {
                Message = loginResponse.Message,
                Token = loginResponse.Token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during login: {ex.Message}");
        }
    }

    [HttpPost("renew")]
    public async Task<ActionResult> RenewToken()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("User not found.");

            var userName = await _accountInfoRepository.GetUserName(userId);

            var token = await _accountInfoRepository.RenewToken(userName);

            if (token == null || string.IsNullOrEmpty(token.Token))
                return BadRequest(token?.Message ?? "An unknown error occurred.");

            return Ok(new
            {
                Message = "Token renewed successfully.",
                Token = token
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred during token renewal: {ex.Message}");
        }
    }



    // POST: /account/logout
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var result = await _accountInfoRepository.LogoutUser();
            if (result == null)
                return StatusCode(500, "An unknown error occurred during logout.");

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
                Id = claim!.Id,
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
