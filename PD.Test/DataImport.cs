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
using PD.Test.Db;
using System.IO;

namespace PD.Test
{
    public class DataImport
    {
        [Fact]
        public void ImportFacultyData()
        {
            ApplicationDbContext db = new SqlServerDb().Db;
            ImportService ds = new ImportService(db);

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts 2016 Faculty FSO Salary Adjustment Report with ChartString.xlsx";
            string worksheet2015_16 = "ARC Academic Salary Adj2015 16";
            Assert.True(File.Exists(dataFile));
            ds.InjestFacultySalaryAdjustmentData(dataFile, worksheet2015_16, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016, 06, 30).Date, new DateTime(2015, 07, 02).Date, new DateTime(2017, 06, 30).Date);
        }
    }
}
