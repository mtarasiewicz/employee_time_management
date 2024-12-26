namespace employee_time_management.Models;

public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public int HoursWorked { get; set; }
    
}