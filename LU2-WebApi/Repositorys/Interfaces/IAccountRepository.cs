public interface IAccountRepository
{
    Task<Result?> RegisterUser(AccountRequest request);
    Task<Result?> LoginUser(LoginRequest request);
    Task<Result?> LogoutUser();
    Task<string> GetUserName(Guid userId);
    Task<IEnumerable<UserClaimDTO?>> GetUserClaims(Guid userId);
}
