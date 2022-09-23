using Fandom_Project.Data;
using Fandom_Project.Repository;
using Fandom_Project.Repository.Interfaces;
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

            // MySQL database on Azure
            //connectionStrBuilder.Add("Server", config["Fandom-Project:DbHost"]);
            //connectionStrBuilder.Add("UserID", config["Fandom-Project:DbUser"]);
            //connectionStrBuilder.Add("Password", config["Fandom-Project:DbPassword"]);
            //connectionStrBuilder.Add("Database", config["Fandom-Project:DbName"]);

            //// MySQL database on localhost
            connectionStrBuilder.Add("Server", "localhost");
            connectionStrBuilder.Add("UserID", "root");
            connectionStrBuilder.Add("Password", "admin");
            connectionStrBuilder.Add("Database", "fandom-project");

            services.AddDbContext<FandomContext>(options => options.UseMySql(connectionStrBuilder.ConnectionString, ServerVersion.AutoDetect(connectionStrBuilder.ConnectionString)));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
