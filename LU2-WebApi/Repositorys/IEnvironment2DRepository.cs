using System.Data;
using Dapper;
using LU2_WebApi.Models;
using Microsoft.Data.SqlClient;

namespace LU2_WebApi.Repositorys
{
    public class IEnvironment2DRepository
    {
        private readonly string _connectionString;

        public IEnvironment2DRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Environment2D>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Environment2D";
                return await connection.QueryAsync<Environment2D>(query);
            }
        }

        public async Task<Environment2D?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Environment2D WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<Environment2D>(query, new { Id = id });
            }
        }

        public async Task<Environment2D> AddAsync(Environment2D environment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Environment2D (Name, MaxLength, MaxHeight)
                    VALUES (@Name, @MaxLength, @MaxHeight);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                int newId = await connection.QuerySingleAsync<int>(query, environment);
                environment.Id = newId;
                return environment;
            }
        }

        public async Task<bool> UpdateAsync(int id, Environment2D updatedEnvironment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Environment2D 
                    SET Name = @Name, MaxLength = @MaxLength, MaxHeight = @MaxHeight 
                    WHERE Id = @Id";

                int rowsAffected = await connection.ExecuteAsync(query, new
                {
                    updatedEnvironment.Name,
                    updatedEnvironment.MaxLength,
                    updatedEnvironment.MaxHeight,
                    Id = id
                });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM Environment2D WHERE Id = @Id";
                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
        }
    }
}
