using DotNetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _configuration;

        public DataContextEF(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // We need DBSet to map our model back to our 
        // Table for each Model we want to modify/alter
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSalary> UserSalary { get; set; }
        public virtual DbSet<UserJobInfo> UserJobInfo { get; set; }

        // tell EF where our Tables are in DB
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TutorialAppSchema"); // set the schema to our schema
            // Map Models to our Tables w/ primary key
            modelBuilder.Entity<User>()
            .ToTable("Users", "TutorialAppSchema") //using ToTable() as Model is User but Table is Users
            .HasKey(u => u.UserId);

            modelBuilder.Entity<UserSalary>()
            .HasKey(u => u.UserId);

            modelBuilder.Entity<UserJobInfo>()
            .HasKey(u => u.UserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // check if already configured
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
                optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }
    }
}