public interface IShareRepository
{
    Task<Result> ShareSceneWithUser(Guid userId, string sharedUserName, Guid worldId);
    Task<List<ShareDTO>> GetAllSharedEnvironments(Guid userId);

}
