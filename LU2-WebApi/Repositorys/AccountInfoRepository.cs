using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class AccountInfoRepository : IAccountInfoRepository
{
    private readonly string _connectionString;

    public AccountInfoRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<IEnumerable<UserClaimDto>> GetClaimsByUserIdAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);

        var query = @"
            SELECT 
                uc.Id, 
                uc.UserId, 
                uc.ClaimType, 
                uc.ClaimValue
            FROM 
                auth.AspNetUserClaims uc
            WHERE 
                uc.UserId = @UserId";

        var claims = await connection.QueryAsync<UserClaimDto>(query, new { UserId = userId });
        return claims;
    }
}
