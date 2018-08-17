using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Configuration;
using System.Linq;

namespace PD.Test
{
    public class DataImport
    {
        private ApplicationDbContext Db;

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        public DataImport()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")));

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"));

            Db = new ApplicationDbContext(builder.Options);
            //Db.Database.Migrate();
        }

        [Fact]
        public void ImportFacultyData()
        {
            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts_2016_Faculty_FSO_Salary_Adjustment_Report_with_ChartString.xlsx";

            var departments = Db.Departments.ToList();
            var chartStrings = Db.ChartStrings.ToList();

            //var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .UseApplicationServiceProvider(new Se)
            //          .UseInMemoryDatabase(Guid.NewGuid().ToString())
            //          .Options;
            //mDb = new ApplicationDbContext(options);

            //ApplicationDbContext db = new ApplicationDbContext(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //DatabaseContext ctx = new DatabaseContext();
            //DataService ds = new DataService()
            //DataService.ImportData(dataFile);
        }
    }
}
