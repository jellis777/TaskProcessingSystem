
using Microsoft.EntityFrameworkCore;
using TaskProcessing.Api.Models;

namespace TaskProcessing.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<TaskProcessingLog> TaskProcessingLogs => Set<TaskProcessingLog>();
    }
}