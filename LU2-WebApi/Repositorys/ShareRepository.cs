using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

public class ShareRepository : IShareRepository
{
    private readonly string _connectionString;

    public ShareRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    private async Task<IDbConnection> GetConnection()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    public async Task<Result> ShareSceneWithUser(Guid userId, string sharedUserName, Guid environmentId)
    {
        try
        {
            using var connection = await GetConnection();
        
            var query = "SELECT CAST(Id AS UNIQUEIDENTIFIER) FROM auth.AspNetUsers WHERE UserName = @UserName";
            var sharedUser = await connection.QueryFirstOrDefaultAsync<Guid?>(query, new { UserName = sharedUserName });

            if (!sharedUser.HasValue)
            {
                return Result.Failure("Delen niet gelukt");
            }

            var checkShareQuery = @"
                SELECT COUNT(1)
                FROM dbo.Shares
                WHERE SharedUserId = @SharedUserId AND EnvironmentId = @EnvironmentId";

            var existingShare = await connection.QueryFirstOrDefaultAsync<int>(checkShareQuery, new
            {
                UserId = userId,
                SharedUserId = sharedUser.Value,
                EnvironmentId = environmentId
            });

            if (existingShare > 0)
            {
                return Result.Failure("Delen niet gelukt");
            }

            var insertQuery = @"
                INSERT INTO dbo.Shares (SharedUserId, EnvironmentId)
                VALUES (@SharedUserId, @EnvironmentId)";

            await connection.ExecuteAsync(insertQuery, new { SharedUserId = sharedUser.Value, EnvironmentId = environmentId });

            return Result.Success("Delen gelukt");
        }
        catch (Exception)
        {
            return Result.Failure("Delen niet gelukt");
        }
    }

    public async Task<List<ShareDTO>> GetAllSharedEnvironments(Guid userId)
    {
        try
        {
            var query = @"
            SELECT e.Id AS EnvironmentId, e.Name AS EnvironmentName
            FROM dbo.Shares s
            JOIN dbo.Environment e ON s.EnvironmentId = e.Id
            WHERE s.SharedUserId = @UserId";

            using var connection = await GetConnection();
            var sharedWorlds = await connection.QueryAsync<ShareDTO>(query, new { UserId = userId });

            return sharedWorlds.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while fetching shared worlds: {ex.Message}");
        }
    }



}
