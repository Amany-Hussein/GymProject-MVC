using GymProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Context
{
    public class GymDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer
            ("Server = . ; DataBase = GymDb , Trusted_Connection = true ; TrustedServerCertificate = true");
        }

        public DbSet<Plan> Plans { get; set; }

    }
}
