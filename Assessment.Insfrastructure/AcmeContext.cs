using Assessment.Domain.Entity;
using Assessment.Infrastructure.Maps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assessment.Infrastructure
{
    public class AcmeContext : DbContext
    {
        public AcmeContext(DbContextOptions<AcmeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Add table maps
            builder.ApplyConfiguration(new EmployeeMessageMap());
            base.OnModelCreating(builder);
        }

        public DbSet<EmployeeMessage> EmployeeMessages { get; set; }
    }
}
