/// <summary>
/// API controller for managing Employee related operations.
/// Provides endpoints for retrieving and creating employee.
/// </summary>
using empservice.Models;
using empservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ServiceBusMessaging;
using System.Collections.Generic;
using System.Text.Json;

namespace empservice.Controllers    
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for EmployeeController.
        /// </summary>
        /// <param name="context">Database context for accessing employee data.</param>
        /// <param name="configuration">Configuration to access application settings.</param>
        public EmployeeController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Simple echo method to verify the API is reachable.
        /// </summary>
        /// <returns>Returns a confirmation message from the server.</returns>
        [HttpGet("echo")]
        public IActionResult Echo()
        {
            // Return the message that was received in the query string
            return Ok("Echo from server.");
        }

        /// <summary>
        /// Retrieves all employees from the database.
        /// </summary>
        /// <returns>A list of all employees in the database.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            List<Employee> employees = await _context.GetAllEmployees();
            return Ok(employees);
        }

        /// <summary>
        /// Creates a new employee and adds it to the database.
        /// Sends a message with the employee data to the Service Bus.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        /// <returns>Returns the created employee object on success or an error on failure.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest();
            }
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                // Serialize the employee object to JSON
                string messageContent = JsonSerializer.Serialize(employee);
                ServiceBusHelper serviceBusHelper = new ServiceBusHelper(_configuration.GetConnectionString("SBConString"));
                await serviceBusHelper.SendMessageAsync(_configuration["SBTopic"], messageContent);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                // Log the error (you can use a logger here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
