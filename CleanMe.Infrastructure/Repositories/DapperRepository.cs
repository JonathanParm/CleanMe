using CleanMe.Application.ViewModels;
using CleanMe.Domain.Interfaces;
using Dapper;
using System.Data;
using System.Reflection;
using System.Web.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CleanMe.Infrastructure.Repositories
{
    public class DapperRepository : IDapperRepository
    {
        private readonly IDbConnection _dbConnection;

        public DapperRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            return await _dbConnection.ExecuteAsync(sql, parameters, commandType: commandType);
        }

        public async Task<int> ExecuteScalarAsync<TScalar>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            var result = await _dbConnection.ExecuteScalarAsync<TScalar>(sql, parameters, commandType: commandType);
            return Convert.ToInt32(result);
        }

    }
}