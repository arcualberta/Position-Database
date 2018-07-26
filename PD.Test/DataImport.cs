using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PD.Test
{
    public class DataImport
    {
        [Fact]
        public void ImportData()
        {
            //string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts_2016_Faculty_FSO_Salary_Adjustment_Report_with_ChartString.xlsx";

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
