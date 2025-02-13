using System.Data;
using Dapper;
using LU2_WebApi.Models;
using Microsoft.Data.SqlClient;

namespace LU2_WebApi.Repositorys
{
    public class IObject2DRepository
    {
        private readonly string _connectionString;

        public IObject2DRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Object2D>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Object2D";
                return await connection.QueryAsync<Object2D>(query);
            }
        }

        public async Task<Object2D?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Object2D WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<Object2D>(query, new { Id = id });
            }
        }

        public async Task<Object2D> AddAsync(Object2D object2D)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Object2D (PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, Environment2D_Id)
                    VALUES (@PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @Environment2D_Id);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                int newId = await connection.QuerySingleAsync<int>(query, object2D);
                object2D.Id = newId;
                return object2D;
            }
        }

        public async Task<bool> UpdateAsync(int id, Object2D updatedObject2D)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Object2D 
                    SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, 
                        ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer, Environment2D_Id = @Environment2D_Id 
                    WHERE Id = @Id";

                int rowsAffected = await connection.ExecuteAsync(query, new
                {
                    updatedObject2D.PrefabId,
                    updatedObject2D.PositionX,
                    updatedObject2D.PositionY,
                    updatedObject2D.ScaleX,
                    updatedObject2D.ScaleY,
                    updatedObject2D.RotationZ,
                    updatedObject2D.SortingLayer,
                    updatedObject2D.Environment2D_Id,
                    Id = id
                });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "DELETE FROM Object2D WHERE Id = @Id";
                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
        }
    }
}
