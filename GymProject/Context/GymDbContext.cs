using GymProject.FluentConfigration;
using GymProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Context
{
    public class GymDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer
            ("Server=.;Database=GymDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfig());
        }
        public DbSet<Plan> Plans { get; set; }

    }
}
