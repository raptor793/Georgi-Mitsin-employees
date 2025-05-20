using Employees.Server.Models;

namespace Employees.Server.Services
{
    public interface IFileService
    {
        Task<List<Employee>> ReadDataAsync(IFormFile file);
    }
}
