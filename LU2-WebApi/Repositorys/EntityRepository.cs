using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using static Dapper.SqlMapper;

public class EntityRepository : IEntityRepository
{
    private readonly string _connectionString;

    public EntityRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private async Task<IDbConnection> GetConnection()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<IEnumerable<Entity?>> GetEntitiesFromEnvironment(Guid environmentId)
    {
        const string query = @"
            SELECT Id, Prefab_Id, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId, CreatedAt, UpdatedAt
            FROM Entity
            WHERE EnvironmentId = @EnvironmentId";

        using var connection = await GetConnection();
        return await connection.QueryAsync<Entity>(query, new { EnvironmentId = environmentId });
    }

    public async Task<int> CreateEntity(Entity entity)
    {
        const string query = @"
            INSERT INTO Entity (Prefab_Id, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId, CreatedAt, UpdatedAt)
            VALUES (@Prefab_Id, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @EnvironmentId, @CreatedAt, @UpdatedAt)";

        using var connection = await GetConnection();
        return await connection.ExecuteAsync(query, entity);
    }

    public async Task<Entity?> GetEntityById(Guid entityId)
    {
        const string query = @"
            SELECT Id, Prefab_Id, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId, CreatedAt, UpdatedAt
            FROM Entity
            WHERE Id = @EntityId";

        using var connection = await GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Entity>(query, new { EntityId = entityId });
    }

    public async Task<int> UpdateEntity(Entity entity)
    {
        const string query = @"
        UPDATE Entity
        SET PositionX = @PositionX,
            PositionY = @PositionY,
            ScaleX = @ScaleX,
            ScaleY = @ScaleY,
            RotationZ = @RotationZ,
            SortingLayer = @SortingLayer,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id";

        using var connection = await GetConnection();
        return await connection.ExecuteAsync(query, entity);
    }


    public async Task<bool> DeleteEntity(Guid entityId)
    {
        const string query = @"
            DELETE 
            FROM Entity 
            WHERE Id = @EntityId";

        using var connection = await GetConnection();
        return await connection.ExecuteAsync(query, new { EntityId = entityId }) > 0;
    }

    public async Task<bool> DeleteEnvironmentEntities(Guid environmentId)
    {
        const string deleteObjectsQuery = @"
            DELETE 
            FROM Entity
            WHERE EnvironmentId = @EnvironmentId";

        using var connection = await GetConnection();
        return await connection.ExecuteAsync(deleteObjectsQuery, new { EnvironmentId = environmentId }) > 0;
    }

}
