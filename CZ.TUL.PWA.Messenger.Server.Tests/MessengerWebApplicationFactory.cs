using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using CZ.TUL.PWA.Messenger.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using CZ.TUL.PWA.Messenger.Server.Tests.Utilities;

namespace CZ.TUL.PWA.Messenger.Server.Tests
{
    public class MessengerWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //builder.ConfigureAppConfiguration((HostingStartupAttribute, config) =>
            //{
            //    config.SetBasePath(Directory.GetCurrentDirectory())
            //          .AddJsonFile($"{assemblyPath}/Config/appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"{assemblyPath}/Config/appsettings.development.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"{assemblyPath}/Config/secretappsettings.json");
            //});

            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<MessengerContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var _builder = services.AddIdentityCore<User>(o =>
                {
                    // configure identity options
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;
                });
                _builder = new IdentityBuilder(_builder.UserType, typeof(IdentityRole), _builder.Services);
                _builder.AddEntityFrameworkStores<MessengerContext>().AddDefaultTokenProviders();

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MessengerContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<MessengerWebApplicationFactory<TStartup>>>();
                    var userManager = scopedServices.GetRequiredService<UserManager<User>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        TestDataSeeding.SeedTestUser(userManager);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the " +
                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
