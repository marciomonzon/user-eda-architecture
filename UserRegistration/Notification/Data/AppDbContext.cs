﻿using Microsoft.EntityFrameworkCore;
using Notification.Models;

namespace Notification.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<EmailNotification> EmailNotifications { get; set; }
    }
}
