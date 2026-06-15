using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.FluentConfigration
{
    public class TrainerConfig : GymUserConfig<Trainer> , IEntityTypeConfiguration<Trainer>
    {
        public new void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(M => M.CreatedAt)
                   .HasColumnName("HireDate")
                   .HasDefaultValueSql("GetDate()");

            base.Configure(builder);
        }
    }
}
