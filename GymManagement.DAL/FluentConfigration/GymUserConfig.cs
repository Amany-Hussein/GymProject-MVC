using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.FluentConfigration
{
    public class GymUserConfig<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(G => G.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(G => G.Email)
                   .HasMaxLength(100);

            builder.HasIndex(G => G.Email).IsUnique();

            builder.HasIndex(G => G.Phone).IsUnique();

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("EmailCheck", "Email Like '_%@_%._%'");
                TB.HasCheckConstraint("PhoneCheck", "Phone Like '010%' or Phone Like '011%' or Phone Like '012%' or Phone Like '015%' ");
            });

            builder.OwnsOne(G => G.Address , Address =>
            {
                Address.Property(G => G.Street).HasColumnName("Street");
                Address.Property(G => G.City).HasColumnName("City");
                Address.Property(G => G.BuildingNumber).HasColumnName("BuildingNumber");

            }
            );

        }
    }
}
