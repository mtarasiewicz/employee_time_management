using employee_time_management.Data;
using employee_time_management.Interfaces;
using employee_time_management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace employee_time_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeesRepository employeesRepository, ITimeEntriesRepository timeEntriesRepository) : ControllerBase
    {
        #region Employee Operations
       
        /// <summary>
        /// Asynchronous method to retrieve all employees.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to fetch all employees from the data source asynchronously.
        /// It handles exceptions by returning a BadRequest with the exception message.
        /// </remarks>
        /// <returns>
        /// An ActionResult containing a list of all employees. 
        /// On success, returns an Ok result with the employees list. 
        /// On failure, returns a BadRequest result with the exception message.
        /// </returns>
        /// <exception cref="System.Exception">
        /// Thrown when there is an error retrieving the employees from the repository.
        /// </exception>
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            try
            {
                var employees = await employeesRepository.GetAllEmployeesAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Asynchronous method to add a new employee.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to add a new employee to the data source asynchronously.
        /// After successfully adding the employee, it returns a CreatedAtAction result with the location of the newly created employee resource.
        /// </remarks>
        /// <param name="employee">The Employee object to be added.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a CreatedAtAction result with the URI of the newly created employee. 
        /// On failure, returns an appropriate error result.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            var id = await employeesRepository.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = id }, new { id });
        }

        /// <summary>
        /// Asynchronous method to retrieve an employee by their ID.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to fetch an employee from the data source asynchronously based on the provided ID.
        /// If no employee is found with the given ID, it returns a NotFound result.
        /// </remarks>
        /// <param name="id">The unique identifier of the employee to be retrieved.</param>
        /// <returns>
        /// An ActionResult containing the employee object. 
        /// On success, returns an Ok result with the employee object. 
        /// On failure, returns a NotFound result if the employee does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(Guid id)
        {
            var employee = await employeesRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        /// <summary>
        /// Asynchronous method to update an existing employee.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to update an employee in the data source asynchronously.
        /// It first checks if the provided ID matches the employee's ID, returning a BadRequest if they do not match.
        /// If the update is successful, it returns a NoContent result. If the employee does not exist, it returns a NotFound result.
        /// </remarks>
        /// <param name="id">The unique identifier of the employee to be updated.</param>
        /// <param name="employee">The Employee object containing updated information.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a NoContent result indicating the update was successful. 
        /// On failure, returns a BadRequest result if the IDs do not match, or a NotFound result if the employee does not exist.
        /// </returns>
        /// <exception cref="System.Exception">
        /// Thrown when there is an error updating the employee in the repository.
        /// </exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }
            var success = await employeesRepository.UpdateEmployeeAsync(employee);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        
        /// <summary>
        /// Asynchronous method to delete an employee by their ID.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to delete an employee from the data source asynchronously based on the provided ID.
        /// If the deletion is successful, it returns a NoContent result. If the employee does not exist, it returns a NotFound result.
        /// </remarks>
        /// <param name="id">The unique identifier of the employee to be deleted.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a NoContent result indicating the deletion was successful. 
        /// On failure, returns a NotFound result if the employee does not exist.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var success = await employeesRepository.DeleteEmployeeAsync(id);

            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        #endregion
        #region Time entry operations
        /// <summary>
        /// Asynchronous method to create a new time entry for an employee.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to add a new time entry to the data source asynchronously.
        /// It validates the work hours to ensure they are between 1 and 24.
        /// It also checks if a time entry for the specified date already exists for the employee, returning a Conflict result if so.
        /// </remarks>
        /// <param name="employeeId">The unique identifier of the employee for whom the time entry is being created.</param>
        /// <param name="timeEntry">The TimeEntry object containing the details of the time worked.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a CreatedAtAction result with the URI of the newly created time entry. 
        /// On failure, returns a BadRequest result if the work hours are invalid, or a Conflict result if a time entry already exists for the given date.
        /// </returns>
        /// <exception cref="System.Exception">
        /// Thrown when there is an error creating the time entry in the repository.
        /// </exception>
        [HttpPost("{employeeId}/time-entries")]
        public async Task<IActionResult> CreateTimeEntry([FromRoute] Guid employeeId, [FromBody] TimeEntry timeEntry)
        {
            if (timeEntry.HoursWorked < 1 || timeEntry.HoursWorked > 24)
            {
                return BadRequest("Work hours must be between 1 and 24.");
            }
            var existingEntry = await timeEntriesRepository.GetTimeEntryByDateAsync(employeeId, timeEntry.Date);
            if (existingEntry != null)
            {
                return Conflict("Time entry for this date already exists.");
            }
    
            var result = await timeEntriesRepository.AddTimeEntryAsync(employeeId, timeEntry);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeId }, result);
        }

        /// <summary>
        /// Asynchronous method to retrieve all time entries for a specific employee.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to fetch all time entries from the data source asynchronously based on the provided employee ID.
        /// </remarks>
        /// <param name="employeeId">The unique identifier of the employee whose time entries are being retrieved.</param>
        /// <returns>
        /// An ActionResult containing a list of time entries for the specified employee. 
        /// On success, returns an Ok result with the list of time entries.
        /// </returns>
        [HttpGet("{employeeId}/time-entries")]
        public async Task<ActionResult<List<TimeEntry>>> GetTimeEntries(Guid employeeId)
        {
            var timeEntries = await timeEntriesRepository.GetTimesEntriesByEmployeeIdAsync(employeeId);
            return Ok(timeEntries);
        }

        /// <summary>
        /// Asynchronous method to retrieve a specific time entry for an employee by their entry ID.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to fetch a time entry from the data source asynchronously based on the provided employee ID and entry ID.
        /// If no time entry is found with the given IDs, it returns a NotFound result.
        /// </remarks>
        /// <param name="employeeId">The unique identifier of the employee whose time entry is being retrieved.</param>
        /// <param name="entryId">The unique identifier of the time entry to be retrieved.</param>
        /// <returns>
        /// An ActionResult containing the time entry object. 
        /// On success, returns an Ok result with the time entry object. 
        /// On failure, returns a NotFound result if the time entry does not exist.
        /// </returns>
        [HttpGet("{employeeId}/time-entries/{entryId}")]
        public async Task<ActionResult<TimeEntry>> GetTimeEntryById(Guid employeeId, Guid entryId)
        {
            var timeEntry = await timeEntriesRepository.GetTimeEntryByIdAsync(employeeId, entryId);
            if (timeEntry == null)
            {
                return NotFound();
            }
            return Ok(timeEntry);
        }

        /// <summary>
        /// Asynchronous method to update an existing time entry for an employee.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to update a time entry in the data source asynchronously based on the provided employee ID and entry ID.
        /// It assigns the entry ID to the time entry object before updating.
        /// If the update is successful, it returns a NoContent result. If the time entry does not exist, it returns a NotFound result.
        /// </remarks>
        /// <param name="employeeId">The unique identifier of the employee whose time entry is being updated.</param>
        /// <param name="entryId">The unique identifier of the time entry to be updated.</param>
        /// <param name="timeEntry">The TimeEntry object containing updated information.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a NoContent result indicating the update was successful. 
        /// On failure, returns a NotFound result if the time entry does not exist.
        /// </returns>
        [HttpPut("{employeeId}/time-entries/{entryId}")]
        public async Task<IActionResult> UpdateTimeEntry(Guid employeeId, Guid entryId, [FromBody] TimeEntry timeEntry)
        {
            timeEntry.Id = entryId;
            var success = await timeEntriesRepository.UpdateTimeEntryAsync(employeeId, timeEntry);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Asynchronous method to delete a specific time entry for an employee by their entry ID.
        /// </summary>
        /// <remarks>
        /// This method uses the repository pattern to delete a time entry from the data source asynchronously based on the provided employee ID and entry ID.
        /// If the deletion is successful, it returns a NoContent result. If the time entry does not exist, it returns a NotFound result.
        /// </remarks>
        /// <param name="employeeId">The unique identifier of the employee whose time entry is being deleted.</param>
        /// <param name="entryId">The unique identifier of the time entry to be deleted.</param>
        /// <returns>
        /// An IActionResult representing the result of the operation. 
        /// On success, returns a NoContent result indicating the deletion was successful. 
        /// On failure, returns a NotFound result if the time entry does not exist.
        /// </returns>
        [HttpDelete("{employeeId}/time-entries/{entryId}")]
        public async Task<IActionResult> DeleteTimeEntry(Guid employeeId, Guid entryId)
        {
            var success = await timeEntriesRepository.DeleteTimeEntryAsync(entryId, employeeId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        #endregion
    }
}
