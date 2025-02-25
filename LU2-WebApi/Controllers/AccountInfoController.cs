using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

[Authorize]
[ApiController]
[Route("account-info")]
public class AccountInfoController : ControllerBase
{
    private readonly IAccountInfoRepository _accountInfoRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly UserManager<AppUser> _userManager;

    public AccountInfoController(IAccountInfoRepository accountInfoRepository, IAuthenticationService authenticationService, UserManager<AppUser> userManager)
    {
        _accountInfoRepository = accountInfoRepository;
        _authenticationService = authenticationService;
        _userManager = userManager;
    }

    // GET: /account-info/claims
    [HttpGet("claims")]
    [Authorize(Policy = "AccessEntity")]
    public async Task<ActionResult<IEnumerable<UserClaimDto>>> GetClaims()
    {
        try
        {
            var userIdString = _authenticationService.GetCurrentAuthenticatedUserId();

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized("User is not authenticated or invalid user ID.");

            var claims = await _accountInfoRepository.GetClaimsByUserIdAsync(userId);

            if (claims == null || !claims.Any())
                return NotFound("No claims found for the user.");

            var claimDtos = claims.Select(claim => new UserClaimDto
            {
                Id = claim.Id,
                UserId = claim.UserId.ToString(),
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            }).ToList();

            return Ok(claimDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching user claims: {ex.Message}");
        }
    }
}


