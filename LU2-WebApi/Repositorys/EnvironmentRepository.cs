using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
public class EnvironmentRepository : IEnvironmentRepository
{
    private readonly string _connectionString;
    public EnvironmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private async Task<IDbConnection> GetConnection()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<IEnumerable<Environment?>> GetEnvironmentsFromUser(Guid userId)
    {
        const string query = @"
            SELECT Id, Name, MaxLength, MaxHeight, CreatedAt, UpdatedAt, CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, EnvironmentType
            FROM dbo.Environment 
            WHERE UserId = @UserId";

        using var connection = await GetConnection();
        return await connection.QueryAsync<Environment>(query, new { UserId = userId });
    }

    public async Task<Environment?> GetEnvironmentById(Guid environmentId, Guid userId)
    {
        const string query = @"
        SELECT e.Id, e.Name, e.MaxLength, e.MaxHeight, e.CreatedAt, e.UpdatedAt, e.EnvironmentType, u.UserName AS OriginalUserName
        FROM dbo.Environment e
        LEFT JOIN dbo.Shares s ON e.Id = s.EnvironmentId
        LEFT JOIN auth.AspNetUsers u ON e.UserId = u.Id
        WHERE (e.Id = @EnvironmentId AND e.UserId = @UserId) OR s.SharedUserId = @UserId";

        using var connection = await GetConnection();
        return await connection.QueryFirstOrDefaultAsync<Environment>(query, new { EnvironmentId = environmentId, UserId = userId });
    }


    public async Task<Result?> CreateEnvironment(Environment environment)
    {
        using var connection = await GetConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            var parameters = new { environment.UserId, environment.Name };

            const string checkQuery = @"
                SELECT 
                    (SELECT COUNT(*) FROM dbo.Environment WHERE UserId = @UserId) AS EnvironmentCount,
                    (SELECT COUNT(*) FROM dbo.Environment WHERE UserId = @UserId AND Name = @Name) AS ExistingNameCount";


            var (environmentCount, existingNameCount) = await connection.QuerySingleAsync<(int, int)>(checkQuery, parameters, transaction);

            if (environmentCount >= 5)
                return Result.Failure("User cannot have more than 5 environments.");
            if (existingNameCount > 0)
                return Result.Failure("An environment with this name already exists.");

            const string insertQuery = @"
                INSERT INTO dbo.Environment (Id, Name, MaxLength, MaxHeight, CreatedAt, UserId, EnvironmentType) 
                VALUES (@Id, @Name, @MaxLength, @MaxHeight, @CreatedAt, @UserId, @EnvironmentType)";

            var result = await connection.ExecuteAsync(insertQuery, environment, transaction);
            transaction.Commit();

            return result > 0 ? Result.Success() : Result.Failure("Failed to create environment.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result.Failure($"An error occurred: {ex.Message}");
        }
    }

    public async Task<Result?> DeleteEnvironment(Guid environmentId, Guid userId)
    {
        using var connection = await GetConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string checkQuery = @"
                SELECT COUNT(*) FROM dbo.Environment WHERE Id = @EnvironmentId AND UserId = @UserId";

            var existingCount = await connection.ExecuteScalarAsync<int>(checkQuery, new { EnvironmentId = environmentId, UserId = userId }, transaction);

            if (existingCount == 0)
                return Result.Failure("Environment not found or does not belong to the user.");

            var entityRepository = new EntityRepository(_connectionString);
            await entityRepository.DeleteEnvironmentEntities(environmentId);

            const string deleteQuery = @"
                DELETE FROM dbo.Environment 
                WHERE Id = @EnvironmentId AND UserId = @UserId";

            var result = await connection.ExecuteAsync(deleteQuery, new { EnvironmentId = environmentId, UserId = userId }, transaction);

            transaction.Commit();

            return result > 0 ? Result.Success() : Result.Failure("Failed to delete environment.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result.Failure($"An error occurred: {ex.Message}");
        }
    }
}
