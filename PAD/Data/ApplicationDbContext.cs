using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PAD.Models;
using PAD.Models.ChartFields;
using PAD.Models.SalaryScales;

namespace PAD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Chart Fields and Chart Strings
        public DbSet<ChartField> ChartFields { get; set; }
        public DbSet<ChartString> ChartStrings { get; set; }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<BusinessUnit> BusinessUnits { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<DeptID> DeptIDs { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        #endregion

        public DbSet<PositionAccount> PositionAccounts { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PersonPosition> PersonPositions { get; set; }
        public DbSet<SalaryScale> SalaryScales { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Department> Departments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Defininig many-to-many relationship between the ChartField and
            // ChartFieldString models via the ChartFiled2ChartFiledString 
            // join table model
            builder.Entity<ChartField2ChartStringJoin>()
                .HasKey(join => new { join.ChartFieldId, join.ChartStringId });

            builder.Entity<ChartField2ChartStringJoin>()
                    .HasOne(join => join.ChartString)
                    .WithMany(cfs => cfs.ChartFields)
                    .HasForeignKey(join => join.ChartStringId);

            builder.Entity<ChartField2ChartStringJoin>()
                .HasOne(join => join.ChartField)
                .WithMany(cf => cf.ChartStrings)
                .HasForeignKey(join => join.ChartFieldId);
        }
    }
}
