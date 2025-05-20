using Employees.Server.Models;

namespace Employees.Server.Services
{
    public class EmployeeService : IEmployeeService
    {
        public List<EmployeePair> CalculatePairWorks(List<Employee> records)
        {
            var result = new List<EmployeePair>();

            var groupedByProject = records.GroupBy(r => r.ProjectID);

            foreach (var projectGroup in groupedByProject)
            {
                List<Employee> list = projectGroup.ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = i + 1; j < list.Count; j++)
                    {
                        Employee employee1 = list[i];
                        Employee employee2 = list[j];

                        int overlapDays = CalculateOverlapDays(employee1.DateFrom, employee1.DateTo, employee2.DateFrom, employee2.DateTo);

                        if (overlapDays > 0)
                        {
                            result.Add(new EmployeePair
                            {
                                EmployeeId1 = Math.Min(employee1.EmployeeId, employee2.EmployeeId),
                                EmployeeId2 = Math.Max(employee1.EmployeeId, employee2.EmployeeId),
                                ProjectId = employee1.ProjectID,
                                DaysWorked = overlapDays
                            });
                        }
                    }
                }
            }

            return result.OrderByDescending(r => r.DaysWorked).ToList();
        }

        private int CalculateOverlapDays(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            DateTime overlapStart = start1 > start2 ? start1 : start2;

            DateTime overlapEnd = end1 < end2 ? end1 : end2;

            if (overlapStart >= overlapEnd)
            {
                return 0;
            }

            return (overlapEnd - overlapStart).Days;
        }
    }
}
