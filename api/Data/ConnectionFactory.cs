using System.Data;
using employee_time_management.Interfaces;
using Npgsql;

namespace employee_time_management.Data;

public class ConnectionFactory(string? connectionString) : IConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}