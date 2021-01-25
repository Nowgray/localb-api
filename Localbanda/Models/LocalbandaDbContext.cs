using Localbanda.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static Localbanda.Helpers.enumeration;
using static Localbanda.Helpers.GNF;

namespace Localbanda.Models
{
    public class LocalbandaDbContext : DbContext
    {
        public LocalbandaDbContext()
        {
        }

        public LocalbandaDbContext(DbContextOptions<LocalbandaDbContext> options)
           : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersPostSync> UsersPostSync { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<EmailTemplates> EmailTemplates { get; set; }
        public DbSet<Email_Notification> Email_Notification { get; set; }
        public DbSet<ExceptionLog> ExceptionLog { get; set; }
        public DbSet<Version> Version { get; set; }
        public DbSet<UserSession> UserSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsersPostSync>().HasKey(table => new
            {
                table.User_Id,
                table.Post_Id,
            });
        }
    }
}
