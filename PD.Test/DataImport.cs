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
using Microsoft.AspNetCore.DataProtection;
using PD.Services.Projections;

namespace PD.Test
{
    public class DataImport
    {
        [Fact]
        public void ImportFacultyData2015_16()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;
            ImportService ds = new ImportService(db);

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts 2016 Faculty FSO Salary Adjustment Report with ChartString.xlsx";
            string worksheet2015_16 = "ARC Academic Salary Adj2015 16";
            Assert.True(File.Exists(dataFile));
            ds.InjestFacultySalaryAdjustmentData(dataFile, worksheet2015_16, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date);
        }

        [Fact]
        public void ImportFacultyData_2018_19()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;
            ImportService ds = new ImportService(db);

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts_Increment Report_TemplateE_2018_19.xlsx";
            string worksheet = "Faculty";
            Assert.True(File.Exists(dataFile));
            ds.UploadFECData(dataFile,
                worksheet,
                new DateTime(2018, 07, 01), //Current year start date
                new DateTime(2019, 06, 30), //Current year end date
                4, //First data row
                281, //Last data row
                1, //Employee Id column
                'B' - 'A' + 1, //Name column
                'C' - 'A' + 1, // position Number
                'D' - 'A' + 1, //Rank
                'E' - 'A' + 1, //RCD
                'F' - 'A' + 1, //Dept
                'G' - 'A' + 1, //Status
                'H' - 'A' + 1, //Step on scale
                'I' - 'A' + 1, //Current salary
                'J' - 'A' + 1, //Market supplement
                'S' - 'A' + 1, //Merit decision column for the given FEC year
                'T' - 'A' + 1 //Merit reason column for the given FEC year
                 );

        }


        [Fact]
        public void ProjectFacultySalaries()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;

            FacultyProjectionService srv = new FacultyProjectionService(db);
            srv.ProjectSalaries(new DateTime(2016, 7, 1).Date);
        }
    }
}
