using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Services;
using PD.Test.Db;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PD.Test
{
    public class DataServiceTest
    {
        [Fact]
        public void ValidateSalaryCalculations()
        {
            ApplicationDbContext db = new SqlServerDb().Db;
            DataService ds = new DataService(db);

            ds.ComputeFacultySalary(1, "2015/16", "2016/17");

            decimal salary = ds.Salary;
            
        }

        [Fact]
        public void DataProtectionTest()
        {
            DataService ds = new DataService(null);

            var input = "Hello World";
            var enc = ds.DataProtector.Encrypt(input);

            DataService ds2 = new DataService(null);
            var dec = ds2.DataProtector.Decrypt(enc);

            Assert.Equal(input, dec);
        }
    }
}