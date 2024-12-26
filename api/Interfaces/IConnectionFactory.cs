using System.Data;

namespace employee_time_management.Interfaces;

public interface IConnectionFactory
{
    IDbConnection CreateConnection();
}