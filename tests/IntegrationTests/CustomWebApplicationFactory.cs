using System.Data;
using employee_time_management.Data;
using employee_time_management.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace tests.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<NpgsqlConnection>(provider =>
            {
                var connection = new NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=123;Database=employee_management;");
                connection.Open();
                return connection;
            });
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<ITimeEntriesRepository, TimeEntriesRepository>();
        });
    }
    
}
