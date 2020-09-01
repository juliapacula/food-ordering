using System;
using DatabaseStructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Migrator
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, collection) => { ConfigureServices(collection, context.Configuration); });
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<DatabaseContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("Postgres")),
                ServiceLifetime.Singleton);

            serviceCollection.AddSingleton<IHostedService, Migrator>();
        }
    }
}