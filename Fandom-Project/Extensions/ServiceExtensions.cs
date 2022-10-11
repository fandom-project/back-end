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
            //connectionStrBuilder.Add("server", config["fandom-project:dbhost"]);
            //connectionStrBuilder.Add("userid", config["fandom-project:dbuser"]);
            //connectionStrBuilder.Add("password", config["fandom-project:dbpassword"]);
            //connectionStrBuilder.Add("database", config["fandom-project:dbname"]);

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
