using Employees.Server.Models;

namespace Employees.Server.Services
{
    public interface IEmployeeService
    {
        List<EmployeePair> CalculatePairWorks(List<Employee> records);
    }
}
