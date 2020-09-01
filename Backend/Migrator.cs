using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Backend
{
    public class Migrator : IHostedService
    {
        private readonly DatabaseContext _databaseContext;

        public Migrator(
            DatabaseContext databaseContext
        )
        {
            _databaseContext = databaseContext;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _databaseContext.Database.EnsureCreatedAsync(cancellationToken);
            await _databaseContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}