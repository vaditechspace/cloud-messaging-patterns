using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmpClient.Models;

public class EmployeeController : Controller
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructor to initialize HttpClient and set the base address.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to be used for making API calls.</param>
    /// <param name="configuration">The configuration settings.</param>
    public EmployeeController(HttpClient httpClient, IConfiguration configuration)
    {
        // Retrieve the base URL for the API service from the configuration
        string baseUrl = configuration.GetSection("ApiSettings:BaseUrl").Value;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);  // Set the base address for HttpClient
    }

    /// <summary>
    /// Handles GET requests to retrieve and display a list of employees.
    /// </summary>
    /// <returns>A view displaying the list of employees.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Make a GET request to the API service
        var response = await _httpClient.GetAsync("");
        if (response.IsSuccessStatusCode)
        {
            // Deserialize the response content to a list of Employee objects
            var employees = await response.Content.ReadFromJsonAsync<List<Employee>>();
            return View(employees);
        }
        // Return an empty list if the API call is not successful
        return View(new List<Employee>());
    }

    /// <summary>
    /// Handles POST requests to save a new employee.
    /// </summary>
    /// <param name="employee">The employee object to be saved.</param>
    /// <returns>A redirect to the Index action if successful, otherwise returns the Index view with the current list of employees.</returns>
    [HttpPost]
    public async Task<IActionResult> Save(Employee employee)
    {
        if (ModelState.IsValid)
        {
            // Make a POST request to the API service with the employee data
            var response = await _httpClient.PostAsJsonAsync("", employee);
            if (response.IsSuccessStatusCode)
            {
                // Redirect to the Index action if the API call is successful
                return RedirectToAction("Index");
            }
        }
        // Return the Index view with the current list of employees if the model state is invalid or the API call fails
        return View("Index", await _httpClient.GetFromJsonAsync<List<Employee>>(""));
    }

    /// <summary>
    /// Handles POST requests to clear the form and redirect to the Index action.
    /// </summary>
    /// <returns>A redirect to the Index action.</returns>
    [HttpPost]
    public IActionResult Clear()
    {
        // Redirect to the Index action
        return RedirectToAction("Index");
    }
}
