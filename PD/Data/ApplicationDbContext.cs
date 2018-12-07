using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PD.Models;
using PD.Models.ChartFields;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;

namespace PD.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
        public DbSet<PositionAssignment> PositionAssignments { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Speedcode> Speedcodes { get; set; }
        public DbSet<ChartField2ChartStringJoin> ChartField2ChartStringJoins { get; set; }
        public DbSet<ChangeLog> ChangeLog { get; set; }
        public DbSet<PromotionScheme> PromotionSchemes { get; set; }

        //Positions
        public DbSet<Position> Positions { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<Nasa> Nasa { get; set; }
        public DbSet<Apo> Apo { get; set; }


        //Salary Scales
        public DbSet<SalaryScale> SalaryScales { get; set; }
        public DbSet<APOSalaryScale> APOSalaryScales { get; set; }
        public DbSet<FacultySalaryScale> FacultySalaryScales { get; set; }
        public DbSet<NASASalaryScale> NASASalaryScales { get; set; }


        //Compensation Types
        public DbSet<Compensation> Compensations { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Merit> Merits { get; set; }
        public DbSet<Adjustment> Adjustments { get; set; }


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

            ////builder.Entity<ChartField2ChartStringJoin>()
            ////    .HasOne(join => join.ChartField)
            ////    .WithMany(cf => cf.ChartStrings)
            ////    .HasForeignKey(join => join.ChartFieldId);

            builder.Entity<PositionAccount>()
                    .HasOne(join => join.Position)
                    .WithMany(pos => pos.PositionAccounts)
                    .HasForeignKey(join => join.PositionId);

            ////builder.Entity<PositionAccount>()
            ////    .HasOne(join => join.ChartString)
            ////    .WithMany(cs => cs.PositionAccounts)
            ////    .HasForeignKey(join => join.ChartStringId);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder
            //.UseLazyLoadingProxies();
        }
    }
}
