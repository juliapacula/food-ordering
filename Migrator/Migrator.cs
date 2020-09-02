using System;
using System.Threading;
using System.Threading.Tasks;
using DatabaseStructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Migrator.Seeders;

namespace Migrator
{
    public class Migrator : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly DatabaseContext _databaseContext;

        public Migrator(
            IHostApplicationLifetime applicationLifetime,
            DatabaseContext databaseContext
        )
        {
            _applicationLifetime = applicationLifetime;
            _databaseContext = databaseContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _databaseContext.Database.EnsureCreatedAsync(cancellationToken);
            await _databaseContext.Database.MigrateAsync(cancellationToken);
            await _databaseContext.SeedDishesAsync(cancellationToken);
            
            _applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}