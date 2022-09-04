using Fandom_Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Fandom_Project.Extensions
{
    public static class ServiceExtensions
    {        
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
        }

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionStrBuilder = new DbConnectionStringBuilder(); // Documentation for DbConnectionStringBuilder() -> https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbconnectionstringbuilder?view=net-6.0

            connectionStrBuilder.Add("Server", config["Fandom-Project:DbHost"]);
            connectionStrBuilder.Add("UserID", config["Fandom-Project:DbUser"]);
            connectionStrBuilder.Add("Password", config["Fandom-Project:DbPassword"]);
            connectionStrBuilder.Add("Database", config["Fandom-Project:DbName"]);
            
            services.AddDbContext<FandomContext>(options => options.UseMySql(connectionStrBuilder.ConnectionString, ServerVersion.AutoDetect(connectionStrBuilder.ConnectionString)));
        }
    }
}
