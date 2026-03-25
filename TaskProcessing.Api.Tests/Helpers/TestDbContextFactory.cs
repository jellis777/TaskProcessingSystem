
using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Data;

namespace TaskProcessing.Api.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            return new ApplicationDbContext(options);
        }
    }
}