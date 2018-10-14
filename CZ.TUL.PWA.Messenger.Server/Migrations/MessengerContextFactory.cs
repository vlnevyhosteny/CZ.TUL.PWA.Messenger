using System;
using Microsoft.EntityFrameworkCore.Design;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    public class MessengerContextFactory : IDesignTimeDbContextFactory<MessengerContext>
    {
        public IConfiguration Configuration { get; }

        public MessengerContextFactory(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public MessengerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("MessengerDatabase"));

            return new MessengerContext(optionsBuilder.Options);
        }

        protected override void OnModelCreating
    }
}
