using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using Dapper;

public class AccountRepository : IAccountRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly string _connectionString;

    public AccountRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, string connectionString)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<Result?> RegisterUser(AccountRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName))
            return Result.Failure("Username must be provided.");

        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 10)
            return Result.Failure("Password does not meet the requirements.");

        var existingUser = await _userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            return Result.Failure("Username is already in use.");

        var user = new AppUser { UserName = request.UserName };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));

        var claimResult = await _userManager.AddClaimAsync(user, new Claim("entity:read", "true"));
        if (!claimResult.Succeeded)
            return Result.Failure(string.Join(", ", claimResult.Errors.Select(e => e.Description)));

        return Result.Success("Registration successful!");
    }

    public async Task<LoginResponse?> LoginUser(LoginRequest request)
    {
        if (_signInManager.Context.User?.Identity?.IsAuthenticated == true)
            return new LoginResponse
            {
                Message = "User is already logged in."
            };

        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            return new LoginResponse
            {
                Message = "Username and password must be provided."
            };

        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return new LoginResponse
            {
                Message = "Invalid username or password."
            };
        var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, false, false);
        if (!result.Succeeded)
            return new LoginResponse
            {
                Message = "Invalid username or password."
            };

        var tokenGenerator = new JwtTokenGenerator(_connectionString);
        var token = tokenGenerator.GenerateToken(request.UserName);
        return new LoginResponse
        {
            Message = "Login successful!",
            Token = token
        };
    }

    // renew token for user after token expiration
    public Task<LoginResponse?> RenewToken(string userName)
    {
        var tokenGenerator = new JwtTokenGenerator(_connectionString);
        var token = tokenGenerator.GenerateToken(userName);
        return Task.FromResult<LoginResponse?>(new LoginResponse
        {
            Message = "Token renewed!",
            Token = token
        });
    }



    public async Task<Result?> LogoutUser()
    {
        await _signInManager.SignOutAsync();
        return Result.Success("Logout successful!");
    }

    public async Task<IEnumerable<UserClaimDTO?>> GetUserClaims(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = @"
            SELECT uc.Id, uc.UserId, uc.ClaimType, uc.ClaimValue
            FROM auth.AspNetUserClaims uc
            WHERE uc.UserId = @UserId";

        var claims = await connection.QueryAsync<UserClaimDTO>(query, new { UserId = userId });
        return claims;
    }

    public async Task<string> GetUserName(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.UserName ?? string.Empty;
    }
}
