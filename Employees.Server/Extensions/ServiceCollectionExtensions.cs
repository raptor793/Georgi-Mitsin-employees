using Employees.Server.Models.Constants;
using Employees.Server.Services;

namespace Employees.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCorsWithOrigins(this IServiceCollection services, IConfiguration configuration)
        {
            string[]? origins = configuration.GetValue<string>(AppSettings.ALLOWED_ORIGINS)?.Split(',');

            if (origins == null)
            {
                throw new ArgumentNullException(nameof(origins));
            }

            services.AddCors(options =>
            {
                options.AddPolicy(Cors.CORS_NAME, policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
        }
    }
}
