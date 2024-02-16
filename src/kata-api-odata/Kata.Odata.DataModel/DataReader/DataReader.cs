using Dapper;
using Kata.Odata.DataModel.DataReader.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kata.Odata.DataModel.DataReader;

public class DataReader : IDataReader
{

    private readonly string _connectionString;

    public DataReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);


    public IDataReader GetContext(string connectionString)
    {
        return new DataReader(connectionString);
    }

    public IDbConnection CreateConnection(Action<object, SqlInfoMessageEventArgs> printInterceptor)
    {
        var connection = new SqlConnection(_connectionString);
        connection.InfoMessage += new SqlInfoMessageEventHandler(printInterceptor);
        return connection;
    }

    public async Task<IEnumerable<dynamic>> QueryAsync(string sqlCommand, object parameter)
    {
        using var connection = CreateConnection();
        return await connection.QueryAsync(sqlCommand, parameter);
    }

    public async Task<int> CountAsync(string sqlCommand, object? param = null)
    {
        if (!sqlCommand.Contains($"[{nameof(CounterResult.Count)}]"))
            throw new InvalidExpressionException($"{nameof(sqlCommand)} must contains a variable called '[{nameof(CounterResult.Count)}]' to use this method.");

        using var connection = CreateConnection();
        var result = await connection.QueryAsync<CounterResult>(sqlCommand, param);
        return result.FirstOrDefault()?.Count ?? 0;

    }
}
