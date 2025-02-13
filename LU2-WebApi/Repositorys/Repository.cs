using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public Repository(string connectionString, string tableName)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _tableName = typeof(T).Name;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<T>($"SELECT * FROM {_tableName}");
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });
    }

    public async Task<T> AddAsync(T entity)
    {
        using var connection = new SqlConnection(_connectionString);
        var insertQuery = GenerateInsertQuery(entity);
        int newId = await connection.QuerySingleAsync<int>(insertQuery, entity);

        var entityType = typeof(T);
        var idProperty = entityType.GetProperty("Id");
        if (idProperty != null)
        {
            idProperty.SetValue(entity, newId);
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(int id, T entity)
    {
        using var connection = new SqlConnection(_connectionString);
        var updateQuery = GenerateUpdateQuery(entity);
        int rowsAffected = await connection.ExecuteAsync(updateQuery, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        int rowsAffected = await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id = @Id", new { Id = id });
        return rowsAffected > 0;
    }

    private string GenerateInsertQuery(T entity)
    {
        var type = typeof(T);
        var properties = type.GetProperties()
                             .Where(p => p.Name != "Id" && p.GetCustomAttribute<NotMappedAttribute>() == null)
                             .ToList();

        var columnNames = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
        var paramNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        return $"INSERT INTO [{_tableName}] ({columnNames}) VALUES ({paramNames}); SELECT CAST(SCOPE_IDENTITY() as int);";
    }

    private string GenerateUpdateQuery(T entity)
    {
        var type = typeof(T);
        var properties = type.GetProperties()
                             .Where(p => p.Name != "Id" && p.GetCustomAttribute<NotMappedAttribute>() == null)
                             .ToList();

        var setClause = string.Join(", ", properties.Select(p => $"[{p.Name}] = @{p.Name}"));

        return $"UPDATE [{_tableName}] SET {setClause} WHERE Id = @Id;";
    }

}
