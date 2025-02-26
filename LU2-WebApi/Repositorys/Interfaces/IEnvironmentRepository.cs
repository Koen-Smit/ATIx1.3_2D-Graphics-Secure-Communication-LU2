public interface IEnvironmentRepository
{
    Task<IEnumerable<Environment?>> GetEnvironmentsFromUser(Guid userId);
    Task<Environment?> GetEnvironmentById(Guid environmentId, Guid userId);
    Task<Result?> CreateEnvironment(Environment environment);
    Task<Result?> DeleteEnvironment(Guid environmentId, Guid userId);
}
