using Dapper;
using Microsoft.Data.SqlClient;

public class Environment2DRepository : IEnvironment2DRepository
{
    private readonly string _connectionString;

    public Environment2DRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Environment2D>> GetUserEnvironmentsAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "SELECT Id, Name, MaxLength, MaxHeight, CreatedAt, UpdatedAt, CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, EnvironmentType FROM dbo.Environment2D WHERE UserId = @UserId";
        return await connection.QueryAsync<Environment2D>(query, new { UserId = userId });
    }

    public async Task<Environment2D?> GetByNameAsync(Guid userId, string name)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT 
                Id, Name, MaxLength, MaxHeight, CreatedAt, UpdatedAt, 
                CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, EnvironmentType
            FROM dbo.Environment2D 
            WHERE UserId = @UserId AND Name = @Name";

        return await connection.QueryFirstOrDefaultAsync<Environment2D>(query, new { UserId = userId, Name = name });
    }

    public async Task<Environment2D?> GetEnvironmentByIdAsync(Guid environmentId, Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
        SELECT 
            Id, Name, MaxLength, MaxHeight, CreatedAt, UpdatedAt, 
            CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, EnvironmentType
        FROM dbo.Environment2D 
        WHERE Id = @EnvironmentId AND UserId = @UserId";

        return await connection.QueryFirstOrDefaultAsync<Environment2D>(query, new { EnvironmentId = environmentId, UserId = userId });
    }

    public async Task<int> GetEnvironmentCountByUserIdAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT COUNT(*) 
            FROM dbo.Environment2D 
            WHERE CAST(UserId AS UNIQUEIDENTIFIER) = @UserId";

        return await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
    }


    public async Task<bool> CreateEnvironmentAsync(Environment2D environment)
    {
        using var connection = new SqlConnection(_connectionString);

        var countQuery = "SELECT COUNT(*) FROM dbo.Environment2D WHERE UserId = @UserId";
        var environmentCount = await connection.ExecuteScalarAsync<int>(countQuery, new { UserId = environment.UserId });

        if (environmentCount >= 5)
            throw new InvalidOperationException("User cannot have more than 5 environments.");

        var nameCheckQuery = "SELECT COUNT(*) FROM dbo.Environment2D WHERE UserId = @UserId AND Name = @Name";
        var existingNameCount = await connection.ExecuteScalarAsync<int>(nameCheckQuery, new { UserId = environment.UserId, Name = environment.Name });

        if (existingNameCount > 0)
            throw new InvalidOperationException("An environment with this name already exists for this user.");

        var insertQuery = @"
            INSERT INTO dbo.Environment2D (Id, Name, MaxLength, MaxHeight, CreatedAt, UserId, EnvironmentType) 
            VALUES (@Id, @Name, @MaxLength, @MaxHeight, @CreatedAt, @UserId, @EnvironmentType)";

        var result = await connection.ExecuteAsync(insertQuery, environment);
        return result > 0;
    }

    public async Task<bool> DeleteEnvironmentAsync(Guid environmentId, Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var checkQuery = "SELECT COUNT(*) FROM dbo.Environment2D WHERE Id = @EnvironmentId AND UserId = @UserId";
            var existingCount = await connection.ExecuteScalarAsync<int>(
                checkQuery, new { EnvironmentId = environmentId, UserId = userId }, transaction);

            if (existingCount == 0)
            {
                transaction.Rollback();
                return false;
            }

            var deleteObjectsQuery = "DELETE FROM dbo.Object2D WHERE Environment2D_Id = @EnvironmentId";
            await connection.ExecuteAsync(deleteObjectsQuery, new { EnvironmentId = environmentId }, transaction);

            var deleteEnvironmentQuery = "DELETE FROM dbo.Environment2D WHERE Id = @EnvironmentId AND UserId = @UserId";
            var result = await connection.ExecuteAsync(deleteEnvironmentQuery, new { EnvironmentId = environmentId, UserId = userId }, transaction);

            transaction.Commit();
            return result > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<Object2D>> GetObjectsByEnvironment2DIdAsync(Guid environmentId)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = @"
            SELECT 
                Id, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, Environment2D_Id
            FROM 
                Object2D
            WHERE 
                Environment2D_Id = @EnvironmentId";

        var objects = await connection.QueryAsync<Object2D>(query, new { EnvironmentId = environmentId });
        return objects;
    }

    public async Task<int> CreateObject2DAsync(Object2D object2D)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = @"
            INSERT INTO Object2D (PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, Environment2D_Id)
            VALUES (@PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @Environment2D_Id)";

        var result = await connection.ExecuteAsync(query, object2D);
        return result;
    }

    public async Task<Object2D?> GetObjectByIdAsync(Guid objectId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = @"
            SELECT 
                Id, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, Environment2D_Id
            FROM 
                Object2D
            WHERE 
                Id = @ObjectId";
        return await connection.QueryFirstOrDefaultAsync<Object2D>(query, new { ObjectId = objectId });
    }

    public async Task<bool> DeleteObjectAsync(Guid objectId)
    {
        using var connection = new SqlConnection(_connectionString);
        var query = "DELETE FROM Object2D WHERE Id = @ObjectId";
        var result = await connection.ExecuteAsync(query, new { ObjectId = objectId });
        return result > 0;
    }
}
