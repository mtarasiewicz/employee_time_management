using employee_time_management;
using employee_time_management.Data;
using employee_time_management.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Api.xml"));
});
builder.Services.AddScoped<IConnectionFactory>(provider =>
{
 var conf = provider.GetRequiredService<IConfiguration>();
 var connectionString = conf.GetConnectionString("DefaultConnection");
 return new ConnectionFactory(connectionString);
});
builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
builder.Services.AddScoped<ITimeEntriesRepository, TimeEntriesRepository>();
builder.Services.AddControllers();
    
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();

public partial class Program
{
}