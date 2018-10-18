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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CZ.TUL.PWA.Messenger.Server.Tests
{
    public class MessengerWebApplicationFactory<TStartup>
        : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureAppConfiguration((HostingStartupAttribute, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config/appsettings.test.json", optional: true, reloadOnChange: true);
            });

            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();
                    
                services.AddDbContext<MessengerContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var _builder = services.AddIdentityCore<User>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequiredLength = 6;
                });
                _builder = new IdentityBuilder(_builder.UserType, typeof(IdentityRole), _builder.Services);
                _builder.AddEntityFrameworkStores<MessengerContext>().AddDefaultTokenProviders();

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;

                    var db = scopedServices.GetRequiredService<MessengerContext>();

                    var logger = scopedServices
                        .GetRequiredService<ILogger<MessengerWebApplicationFactory<TStartup>>>();
                        
                    db.Database.EnsureCreated();

                    try
                    {
                        TestDataSeeding.SeedTestUser(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the " +
                            "database . Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
