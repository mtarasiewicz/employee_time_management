using employee_time_management.Models;

namespace employee_time_management.Interfaces;

public interface IEmployeesRepository
{
    Task<List<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(Guid id);
    Task<Guid> AddEmployeeAsync(Employee employee);
    Task<bool> UpdateEmployeeAsync(Employee employee);
    Task<bool> DeleteEmployeeAsync(Guid id);
}