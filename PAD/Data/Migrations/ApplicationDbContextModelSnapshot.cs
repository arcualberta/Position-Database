﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using PAD.Data;
using PAD.Models;
using System;

namespace PAD.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("PAD.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("PAD.Models.ChartField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("ChartFields");

                    b.HasDiscriminator<string>("Discriminator").HasValue("ChartField");
                });

            modelBuilder.Entity("PAD.Models.ChartField2ChartStringJoin", b =>
                {
                    b.Property<int>("ChartFieldId");

                    b.Property<int>("ChartStringId");

                    b.HasKey("ChartFieldId", "ChartStringId");

                    b.HasIndex("ChartStringId");

                    b.ToTable("ChartField2ChartStringJoin");
                });

            modelBuilder.Entity("PAD.Models.ChartString", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("ChartStrings");
                });

            modelBuilder.Entity("PAD.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("PAD.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("EmployeeId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("PAD.Models.PersonPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("PersonPositions");
                });

            modelBuilder.Entity("PAD.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CurrentPersonId");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EffectiveDate");

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Number");

                    b.Property<int>("PositionType");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("CurrentPersonId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("PAD.Models.PositionAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ChartStringId");

                    b.Property<int>("PositionId");

                    b.Property<decimal>("Proportion");

                    b.HasKey("Id");

                    b.HasIndex("ChartStringId");

                    b.HasIndex("PositionId");

                    b.ToTable("PositionAccounts");
                });

            modelBuilder.Entity("PAD.Models.SalaryScales.SalaryScale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("ATBPercentatge");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Guid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("SalaryScales");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Account", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Account");

                    b.HasDiscriminator().HasValue("Account");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.BusinessUnit", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("BusinessUnit");

                    b.HasDiscriminator().HasValue("BusinessUnit");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Class", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Class");

                    b.HasDiscriminator().HasValue("Class");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.DeptID", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");

                    b.Property<int>("DepartmentId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("DeptID");

                    b.HasDiscriminator().HasValue("DeptID");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Fund", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Fund");

                    b.HasDiscriminator().HasValue("Fund");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Program", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Program");

                    b.HasDiscriminator().HasValue("Program");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Project", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Project");

                    b.HasDiscriminator().HasValue("Project");
                });

            modelBuilder.Entity("PAD.Models.ChartFields.Sponsor", b =>
                {
                    b.HasBaseType("PAD.Models.ChartField");


                    b.ToTable("Sponsor");

                    b.HasDiscriminator().HasValue("Sponsor");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("PAD.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("PAD.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PAD.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("PAD.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PAD.Models.ChartField2ChartStringJoin", b =>
                {
                    b.HasOne("PAD.Models.ChartField", "ChartField")
                        .WithMany("ChartStrings")
                        .HasForeignKey("ChartFieldId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PAD.Models.ChartString", "ChartString")
                        .WithMany("ChartFields")
                        .HasForeignKey("ChartStringId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PAD.Models.Position", b =>
                {
                    b.HasOne("PAD.Models.PersonPosition", "CurrentPerson")
                        .WithMany("Positions")
                        .HasForeignKey("CurrentPersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PAD.Models.PositionAccount", b =>
                {
                    b.HasOne("PAD.Models.ChartString", "ChartString")
                        .WithMany()
                        .HasForeignKey("ChartStringId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PAD.Models.Position", "Position")
                        .WithMany("Accounts")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PAD.Models.ChartFields.DeptID", b =>
                {
                    b.HasOne("PAD.Models.Department", "Department")
                        .WithMany("DeptIDs")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
