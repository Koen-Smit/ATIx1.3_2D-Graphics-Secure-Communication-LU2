public interface IAccountRepository
{
    Task<Result?> RegisterUser(AccountRequest request);
    Task<LoginResponse?> LoginUser(LoginRequest request);
    Task<LoginResponse?> RenewToken(string userName);
    Task<Result?> LogoutUser();
    Task<string> GetUserName(Guid userId);
    Task<IEnumerable<UserClaimDTO?>> GetUserClaims(Guid userId);
}
