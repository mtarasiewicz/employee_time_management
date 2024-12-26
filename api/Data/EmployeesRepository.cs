using System.Data;
using Dapper;
using employee_time_management.Interfaces;
using employee_time_management.Models;

namespace employee_time_management.Data;

public class EmployeesRepository(IConnectionFactory connectionFactory) : IEmployeesRepository
{
    private IDbConnection Connection => connectionFactory.CreateConnection();
    
    public async Task<List<Employee>> GetAllEmployeesAsync()
    { 
        using var connection = Connection;
        connection.Open();
        var employees = await connection.QueryAsync<Employee>(@"select * from employees");
        return employees.AsList();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
    {
        using var connection = Connection;
        connection.Open();
        var employee = await connection.QueryFirstOrDefaultAsync<Employee>(@"select * from employees where id = @id", new { id });
        return employee;
    }

    public async Task<Guid> AddEmployeeAsync(Employee employee)
    {   
        using var connection = Connection;
        connection.Open();
        var query = "INSERT INTO employees (firstname, lastname, email) VALUES (@FirstName, @LastName, @Email) returning id";
        var id = await connection.QueryFirstOrDefaultAsync<Guid>(query, new
        {
            employee.FirstName,
            employee.LastName,
            employee.Email
        });
        return id;
    }

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        using var connection = Connection;
        var checkQuery = "SELECT COUNT(1) FROM employees WHERE id = @Id";
        var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { employee.Id });

        if (exists == 0)
        {
            return false;
        }

        var query = """
                    
                                UPDATE employees
                                SET firstname = @FirstName, lastname = @LastName, email = @Email
                                WHERE id = @Id
                    """;
        var rowsAffected = await connection.ExecuteAsync(query, new
        {
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email
        });

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteEmployeeAsync(Guid id)
    {
        using var connection = Connection;
        var checkQuery = "SELECT COUNT(1) FROM employees WHERE id = @Id";
        var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = id });
        if (exists == 0)
        {
            return false;
        }

        var query = "DELETE FROM employees WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }
}