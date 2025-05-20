using Employees.Server.Models;
using Employees.Server.Models.Exceptions;
using Employees.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IFileService fileService, 
            IEmployeeService employeeService,
            ILogger<EmployeeController> logger)
        {
            _fileService = fileService;
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CalculatePairs(IFormFile file)
        {
            try
            {
                List<Employee> employees = await _fileService.ReadDataAsync(file);
                List<EmployeePair> employeePairs = _employeeService.CalculatePairWorks(employees);

                return Ok(employeePairs);
            }
            catch (InvalidFileException ex)
            {
                return ex.StatusCode == StatusCodes.Status400BadRequest 
                    ? BadRequest(ex.Message) : StatusCode(StatusCodes.Status413PayloadTooLarge, ex.Message);
            }
            catch (Exception) 
            { 
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
