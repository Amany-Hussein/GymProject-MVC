using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagement.DAL.FluentConfigration
{
    public class MemberConfig : GymUserConfig<Member> , IEntityTypeConfiguration<Member>
    {
        public  new void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(M => M.CreatedAt)
                   .HasColumnName("JoinDate")
                   .HasDefaultValueSql("GetDate()");

            base.Configure(builder);
        }
    }
}
