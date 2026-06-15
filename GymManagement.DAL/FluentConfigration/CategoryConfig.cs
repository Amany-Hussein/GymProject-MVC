using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.FluentConfigration
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.CategoryName)
                   .HasMaxLength(50);

            builder.Property(C => C.CreatedAt)
                   .HasDefaultValueSql("GetDate()");

            // Seeding

            builder.HasData(
                new Category { Id = 1, CategoryName = "Cardio" },
                new Category { Id = 2, CategoryName = "Yoga" },
                new Category { Id = 3, CategoryName = "Boxing" },
                new Category { Id = 4, CategoryName = "GeneralFitness" },
                new Category { Id = 5, CategoryName = "CrossFit" }
                );
        }
    }
}
