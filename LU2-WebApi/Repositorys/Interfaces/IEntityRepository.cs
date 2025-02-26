public interface IEntityRepository
{
    Task<IEnumerable<Entity?>> GetEntitiesFromEnvironment(Guid environmentId);
    Task<int> CreateEntity(Entity entity);
    Task<Entity?> GetEntityById(Guid entityId);
    Task<int> UpdateEntity(Entity entity);
    Task<bool> DeleteEntity(Guid entityId);
}
