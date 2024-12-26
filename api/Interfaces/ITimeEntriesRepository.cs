using employee_time_management.Models;

namespace employee_time_management.Interfaces;

public interface ITimeEntriesRepository
{
    Task<List<TimeEntry>> GetTimesEntriesByEmployeeIdAsync(Guid employeeId);
    Task<TimeEntry?> GetTimeEntryByIdAsync(Guid employeeId, Guid entryId);
    Task<TimeEntry?> GetTimeEntryByDateAsync(Guid employeeId, DateTime date);
    Task<Guid> AddTimeEntryAsync(Guid employeeId, TimeEntry timeEntry);
    Task<bool> UpdateTimeEntryAsync(Guid employeeId,TimeEntry timeEntry);
    Task<bool> DeleteTimeEntryAsync(Guid employeeId, Guid entryId);
}