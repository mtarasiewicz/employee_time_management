using System.Data;
using Dapper;
using employee_time_management.Interfaces;
using employee_time_management.Models;

namespace employee_time_management.Data;

public class TimeEntriesRepository(IConnectionFactory connectionFactory) : ITimeEntriesRepository
{
    private readonly IConnectionFactory _connectionFactory = connectionFactory;
    private IDbConnection Connection => _connectionFactory.CreateConnection();
    public async Task<Guid> AddTimeEntryAsync(Guid employeeId,TimeEntry timeEntry)
    {
        using var connection = Connection;
        connection.Open();
        var query = """
                                INSERT INTO timeentries (employeeid, date, hoursworked) 
                                VALUES (@EmployeeId, @Date, @HoursWorked) 
                                RETURNING id
                    """;
        
        var id = await connection.QueryFirstOrDefaultAsync<Guid>(query, new
        {
            employeeId,
            timeEntry.Date,
            timeEntry.HoursWorked
        });

        return id;
    }

    public async Task<List<TimeEntry>> GetTimesEntriesByEmployeeIdAsync(Guid employeeId)
    {
        using var connection = Connection;
        connection.Open();
        var query = "SELECT * FROM timeentries WHERE employeeid = @EmployeeId";
        var times = await connection.QueryAsync<TimeEntry>(query, new { EmployeeId = employeeId });
        return times.AsList();
    }
    

    public async Task<bool> UpdateTimeEntryAsync(Guid employeeId, TimeEntry timeEntry)
    {
        using var connection = Connection;
        connection.Open();
        var query = "UPDATE timeentries SET date = @Date, hoursworked = @HoursWorked WHERE employeeid = @EmployeeId AND id = @EntryId";
        var result = await connection.ExecuteAsync(query, new
        {
            EmployeeId = employeeId,
            EntryId = timeEntry.Id,
            Date = timeEntry.Date,
            HoursWorked = timeEntry.HoursWorked
        });

        return result > 0;
    }

    public async Task<bool> DeleteTimeEntryAsync(Guid id, Guid employeeId)
    {
        using var connection = Connection;
        connection.Open();
        var query = "DELETE FROM timeentries WHERE employeeid = @EmployeeId AND id = @EntryId";
        var result = await connection.ExecuteAsync(query, new { EmployeeId = employeeId, EntryId = id });

        return result > 0;
    }
    
    public async Task<TimeEntry?> GetTimeEntryByIdAsync(Guid employeeId, Guid entryId)
    {
        using var connection = Connection;
        connection.Open();
        var query = "SELECT * FROM timeentries WHERE employeeid = @EmployeeId AND id = @EntryId";
        var result = await connection.QueryFirstOrDefaultAsync<TimeEntry>(query, new { EmployeeId = employeeId, EntryId = entryId });
        return result;
    }

    public async Task<TimeEntry?> GetTimeEntryByDateAsync(Guid employeeId, DateTime date)
    {
        using var connection = Connection;
        connection.Open();
        var query = "SELECT * FROM timeentries WHERE employeeid = @EmployeeId AND date = @Date";
        var result = await connection.QueryFirstOrDefaultAsync<TimeEntry>(query, new { EmployeeId = employeeId, Date = date });
        return result;
    }
}