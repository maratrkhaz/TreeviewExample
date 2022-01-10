using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewExample.Models;

namespace TreeViewExample.Persistence
{
    public class OrgUnitDbContext : DbContext
    {
        public DbSet<OrgUnitBase> OrgUnits { get; set; }

        public DbSet<OrgUnitType> OrgUnitTypes { get; set; }

        public OrgUnitDbContext(DbContextOptions<OrgUnitDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrgUnitBase>()
                .HasDiscriminator<string>("OrgUnitType")
                .HasValue<Company>(nameof(Company))
                .HasValue<SubUnit>(nameof(SubUnit))
                .HasValue<RetStore>(nameof(RetStore));

            modelBuilder.Entity<OrgUnitType>().HasData(
                new OrgUnitType { Id = "1", Code = "cmp", Name = "Company" },
                new OrgUnitType { Id = "2", Code = "sub", Name = "SubUnit" },
                new OrgUnitType { Id = "3", Code = "ret", Name = "RetailStore"}
            );
        }
    }
}
