using System;
using Microsoft.EntityFrameworkCore.Design;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CZ.TUL.PWA.Messenger.Server.Migrations
{
    public class MessengerContextFactory : IDesignTimeDbContextFactory<MessengerContext>
    {
        public MessengerContextFactory()
        {
        }

        public MessengerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MessengerContext>();
            optionsBuilder.

            return new MessengerContext(optionsBuilder.Options);
        }
    }
}
