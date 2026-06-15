using GymProject.FluentConfigration;
using GymProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Context
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) :base(options) 
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfig());
        }
        public DbSet<Plan> Plans { get; set; }

    }
}
