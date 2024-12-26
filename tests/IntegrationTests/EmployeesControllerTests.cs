using System.Net;
using System.Net.Http.Json;
using System.Text;
using employee_time_management.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Npgsql;

namespace tests.IntegrationTests;

public class EmployeesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly NpgsqlConnection _connection;
    private readonly Guid _employeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public EmployeesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _connection = factory.Server.Services.GetRequiredService<NpgsqlConnection>();
    }

    public async Task InitializeAsync()
    {
        await ClearDatabase();
        
        var employee = new Employee
        {
            Id = _employeeId,
            FirstName = "Jan",
            LastName = "T",
            Email = "jan.testowy@example.pl"
        };
        var content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/Employees", content);
        response.EnsureSuccessStatusCode();
        
    }

    public async Task DisposeAsync()
    {
        await ClearDatabase();
    }

    private async Task ClearDatabase()
    {
        using (var transaction = await _connection.BeginTransactionAsync())
        {
            try
            {
                using (var command = new NpgsqlCommand("DELETE FROM employees", _connection, transaction))
                {
                    await command.ExecuteNonQueryAsync();
                }

                using (var command = new NpgsqlCommand("DELETE FROM timeentries", _connection, transaction))
                {
                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    
    [Fact]
    public async Task AddEmployee_ReturnsCreated()
    {
        // Arrange
        var employee = new Employee { FirstName = "Jan", LastName = "Testowy", Email = "jan.testowy2@example.com" };
        var content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Employees", content);

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        await using var command = new NpgsqlCommand("SELECT COUNT(*) FROM employees WHERE firstname = @name", _connection);
        command.Parameters.AddWithValue("@name", employee.FirstName);
        var count = await command.ExecuteScalarAsync();
        Assert.Equal(1, Convert.ToInt32(count));
    }
    [Fact]
    public async Task GetAllEmployees_ReturnsListOfEmployees()
    {
        // Act
        var response = await _client.GetAsync("api/Employees");

        // Assert
        response.EnsureSuccessStatusCode(); 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); 
        
        var employees = JsonConvert.DeserializeObject<List<Employee>>(await response.Content.ReadAsStringAsync());
        Assert.NotNull(employees); 
        Assert.True(employees.Count >= 0); 
    }
}
