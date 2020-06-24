using Assessment.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Infrastructure.Maps
{
    public class EmployeeMessageMap : IEntityTypeConfiguration<EmployeeMessage>
    {

        public void Configure(EntityTypeBuilder<EmployeeMessage> builder)
        {
            builder.ToTable("EmployeeMessage");
            builder.HasKey(x => x.EmployeeMessageId);
            builder.Property(x => x.EmployeeId).HasColumnName("EmployeeId").IsRequired();
            builder.Property(x => x.MessageDate).HasColumnName("MessageDate").IsRequired();
            builder.Property(x => x.CorrespondenceType).HasMaxLength(50).IsUnicode(true);
        }
    }
}
