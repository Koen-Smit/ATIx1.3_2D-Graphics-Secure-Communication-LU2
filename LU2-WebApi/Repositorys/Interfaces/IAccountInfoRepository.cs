public interface IAccountInfoRepository
{
    public Task<IEnumerable<UserClaimDto>> GetClaimsByUserIdAsync(Guid userId);
}
