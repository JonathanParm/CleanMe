using System.Data;

namespace CleanMe.Domain.Interfaces
{
    public interface IDapperRepository
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteScalarAsync<TScalar>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
    }
}