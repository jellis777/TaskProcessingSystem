using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using TaskProcessing.Api.Data;

namespace TaskProcessing.Api.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>))
                .ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            var dbContextOptionsConfigurationDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>))
                .ToList();

            foreach (var descriptor in dbContextOptionsConfigurationDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}