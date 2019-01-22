using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Services;
using PD.Test.Db;
using Xunit;

namespace PD.Test
{
    public class DataServiceTest : PdTestBase
    {
        public DataServiceTest()
        {

        }

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
            ////DataService ds = new DataService(null);

            ////var input = "Hello World";
            ////var enc = ds.DataProtector.Encrypt(input);

            ////DataService ds2 = new DataService(null);
            ////var dec = ds2.DataProtector.Decrypt(enc);

            ////Assert.Equal(input, dec);
            ///

            var dp1 = _serviceProvider.GetService<IPdDataProtector>();
            var input = "Hello World";
            var enc = dp1.Encrypt(input);

            var dp2 = _serviceProvider.GetService<IPdDataProtector>();
            var dec = dp2.Decrypt(enc);

            Assert.NotEqual(dp1.GetHashCode(), dp2.GetHashCode());

            Assert.Equal(input, dec);

        }
    }
}