using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend
{
    class Program
    {
        public static IConfigurationRoot configuration;

        public static void Main()
        {
            CreateHostBuilder().Build().Run();
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, collection) =>
                {
                    CreateConfiguration();
                    ConfigureServices(collection);
                });
        }

        private static void CreateConfiguration()
        {
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddEntityFrameworkNpgsql()
                .AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));
            serviceCollection.AddTransient<QueueHandler>();
        }
    }
}