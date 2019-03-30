using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Models;
using PD.Services;
using PD.Services.Projections;
using PD.Test.Db;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PD.Test
{
    public class DataServiceTest : PdTestBase
    {
        public DataServiceTest()
        {

        }

        [Fact]
        public void DataProtectionTest()
        {
            var dp1 = _serviceProvider.GetService<IPdDataProtector>();
            var input = "Hello World";
            var enc = dp1.Encrypt(input);

            var dp2 = _serviceProvider.GetService<IPdDataProtector>();
            var dec = dp2.Decrypt(enc);

            Assert.Equal(input, dec);
        }

        [Fact]
        public void PersonSaveTest()
        {
            Random rand = new Random();
            var dp1 = _serviceProvider.GetService<IPdDataProtector>();
            var db = _serviceProvider.GetService<ApplicationDbContext>();

            var name = string.Format("Name {0}", rand.Next());
            var emplId = DateTime.Now.Ticks.ToString();
            Person person = new Person() { Name = dp1.Encrypt(name), EmployeeId = emplId };
            db.Persons.Add(person);
            db.SaveChanges();

            var dp2 = _serviceProvider.GetService<IPdDataProtector>();
            var decName = dp2.Decrypt(db.Persons.Where(p => p.EmployeeId == emplId).Select(p => p.Name).FirstOrDefault());

            Assert.Equal(name, decName);

        }

        [Fact]
        public void CreateSalaryScales()
        {
            ImportService ds = _serviceProvider.GetService<ImportService>();

            //Salary Scales
            //=============
            DateTime start, end;

            start = new DateTime(2014, 07, 01);
            end = new DateTime(2015, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 75403, 104827, 2452, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 87656, 131672, 3144, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 109079, 131260, 3697, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 131261, 143836, 3144, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 143837, 0, 2452, 1m, 1.65m, false);

            start = new DateTime(2015, 07, 01);
            end = new DateTime(2016, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 76534, 106402, 2489, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 88971, 133645, 3191, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 110715, 133226, 3752, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 133227, 145990, 3191, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 145991, 0, 2489, 1m, 1.5m, false);

            start = new DateTime(2016, 07, 01);
            end = new DateTime(2017, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 77299, 107467, 2514, 1m, 1m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 89861, 134983, 3223, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 111822, 134561, 3790, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 134562, 147453, 3223, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 147454, 0, 2514, 1m, 1m, false);

            start = new DateTime(2017, 07, 01);
            end = new DateTime(2018, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 78458, 109082, 2552, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 91209, 137003, 3271, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 113499, 136580, 3847, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 136581, 149664, 3271, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 149665, 0, 2552, 1m, 1.5m, false);

            ds.Db.SaveChanges();
        }

        [Fact]
        public void ImportFacultyData2015_16()
        {

            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;
            ImportService ds = _serviceProvider.GetService<ImportService>();

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts 2016 Faculty FSO Salary Adjustment Report with ChartString.xlsx";
            string worksheet2015_16 = "ARC Academic Salary Adj2015 16";
            Assert.True(File.Exists(dataFile));
            ds.InjestFacultySalaryAdjustmentData(dataFile, false, worksheet2015_16, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date);

            CreateSalaryScales();
            ComputeFacultySalaries();
        }

        [Fact]
        public void ComputeFacultySalaries()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;

            FacultyProjectionService srv = _serviceProvider.GetService<FacultyProjectionService>();
            for (int year = 2016; year < 2020; ++year)
            {
                var result = srv.ProjectFacultySalaries(new DateTime(year, 7, 1).Date);
                var errors = result.Errors.Distinct().ToList();
                var successes = result.Successes;
            }
        }

        [Fact]
        public void ImportFacultyData_2018_19()
        {
            ImportService ds = _serviceProvider.GetService<ImportService>();

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts_Increment Report_TemplateE_2018_19.xlsx";
            string worksheet = "Faculty";
            Assert.True(File.Exists(dataFile));
            ds.UploadFECData(dataFile,
                false,
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
        public void ComputetOneEmployeeSalaries()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;

            var dp = _serviceProvider.GetService<IPdDataProtector>();
            string employeeName = "Altamirano-Jimenez,Isabel";
            var employees = db.Persons.Include(p => p.PositionAssignments).ToList();
            var pa_Id = employees
                .Where(empl => dp.Decrypt(empl.Name) == employeeName)
                .FirstOrDefault()
                .PositionAssignments.Select(pa => pa.Id)
                .FirstOrDefault();

            FacultyProjectionService srv = _serviceProvider.GetService<FacultyProjectionService>();
            srv.ComputeSalaries(pa_Id,
                new DateTime(2016, 7, 1).Date,
                new DateTime(2019, 7, 1).Date);
        }

    }
}