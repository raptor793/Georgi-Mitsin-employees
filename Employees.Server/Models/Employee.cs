namespace Employees.Server.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
