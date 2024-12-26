using employee_time_management.Controllers;
using employee_time_management.Interfaces;
using employee_time_management.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace tests.UnitTests;

public class EmployeesControllerTests
{
        private readonly Mock<IEmployeesRepository> _mockRepoEmployee;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockRepoEmployee = new Mock<IEmployeesRepository>();
            Mock<ITimeEntriesRepository> mockRepoTimeEntries = new();
            _controller = new EmployeesController(_mockRepoEmployee.Object, mockRepoTimeEntries.Object);
        }

        [Fact]
        public async Task AddEmployee_ReturnsCreatedAtAction_WhenEmployeeIsAdded()
        {
            // Arrange
            var employee = new Employee { Id = Guid.NewGuid(), FirstName = "Piotr", LastName = "Testowy", Email = "testowy@test.com" };
            _mockRepoEmployee.Setup(repo => repo.AddEmployeeAsync(employee)).ReturnsAsync(employee.Id);

            // Act
            var result = await _controller.AddEmployee(employee);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetEmployeeById), createdAtActionResult.ActionName);
            Assert.Equal(employee.Id, createdAtActionResult.RouteValues?["id"]);
        }

        [Fact]
        public async Task CreateTimeEntry_ReturnsBadRequest_WhenHoursWorkedIsInvalid()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var timeEntry = new TimeEntry { Date = DateTime.UtcNow, HoursWorked = 200 };

            // Act
            var result = await _controller.CreateTimeEntry(employeeId, timeEntry);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Work hours must be between 1 and 24.", badRequestResult.Value);
            }
}