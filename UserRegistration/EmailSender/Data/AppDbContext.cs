using EmailSender.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailSender.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
    }
}
