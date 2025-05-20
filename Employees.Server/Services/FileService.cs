using Employees.Server.Models;
using Employees.Server.Models.Exceptions;
using System.Globalization;
using System.Text;

namespace Employees.Server.Services
{
    public class FileService : IFileService
    {
        public async Task<List<Employee>> ReadDataAsync(IFormFile file)
        {
            ValidateFile(file);

            using var reader = new StreamReader(file.OpenReadStream());
            string content = await reader.ReadToEndAsync();
            var errors = new StringBuilder();
            var records = new List<Employee>();

            string[] lines = content.Split('\n');

            string[] acceptedFormats =
            {
                "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy",
                "yyyy/MM/dd", "dd/MM/yyyy", "M/d/yyyy"
            };

            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(',');

                if (parts.Length < 4)
                {
                    errors.AppendLine($"Invalid line format: {line}");

                    continue;
                }

                if (!int.TryParse(parts[0].Trim(), out int employeeId) ||
                    !int.TryParse(parts[1].Trim(), out int projectId))
                {
                    errors.AppendLine($"Invalid EmpID or ProjectID on line: {line}");

                    continue;
                }

                if (!TryParseDate(parts[2].Trim(), acceptedFormats, out DateTime dateFrom))
                {
                    errors.AppendLine($"Invalid DateFrom: {parts[2]}");

                    continue;
                }

                DateTime dateTo;

                if (parts[3].Trim().ToUpper() == "NULL")
                {
                    dateTo = DateTime.Today;
                }
                else if (!TryParseDate(parts[3].Trim(), acceptedFormats, out dateTo))
                {
                    errors.AppendLine($"Invalid DateTo: {parts[3]}");

                    continue;
                }

                records.Add(new Employee
                {
                    EmployeeId = employeeId,
                    ProjectID = projectId,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                });
            }

            if (errors.Length > 0)
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, errors.ToString());
            }

            return records;
        }

        private bool TryParseDate(string input, string[] formats, out DateTime date)
        {
            return DateTime.TryParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date
            );
        }

        private void ValidateFile(IFormFile file)
        {
            long maxFileSize = 1 * 1024 * 1024; // 1 MB
            string[] AllowedExtensions = { ".csv" };

            if (file == null)
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, "No file to read.");
            }

            if (file.Length == 0)
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, "The file is empty.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(fileExtension))
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, "Only .csv files are allowed.");
            }

            if (file.Length > maxFileSize)
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, "File exceeds the maximum size of 1 MB.");
            }

            if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase) &&
                !file.ContentType.Equals("application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidFileException(StatusCodes.Status400BadRequest, "Invalid MIME type for a CSV file.");
            }
        }
    }
}
