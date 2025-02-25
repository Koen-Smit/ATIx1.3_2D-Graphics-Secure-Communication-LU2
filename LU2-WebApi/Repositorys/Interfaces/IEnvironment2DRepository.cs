using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2D>> GetUserEnvironmentsAsync(Guid userId);
    Task<Environment2D?> GetByNameAsync(Guid userId, string name);
    Task<Environment2D?> GetEnvironmentByIdAsync(Guid environmentId, Guid userId);
    Task<int> GetEnvironmentCountByUserIdAsync(Guid userId);
    Task<bool> CreateEnvironmentAsync(Environment2D environment);
    Task<bool> DeleteEnvironmentAsync(Guid environmentId, Guid userId);
    Task<int> CreateObject2DAsync(Object2D object2D);
    Task<IEnumerable<Object2D>> GetObjectsByEnvironment2DIdAsync(Guid environmentId);
    Task<bool> DeleteObjectAsync(Guid objectId);
    Task<Object2D?> GetObjectByIdAsync(Guid objectId);
}
