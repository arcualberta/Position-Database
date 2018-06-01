using PD.Data;
using PD.Models.ChartFields;
using PD.Services;
using System;
using Xunit;
using System.Linq;
using PD.Models;

namespace PD.Test
{
    public class ChartStringServiceTest
    {
        [Fact]
        public void CreateChartFieldTest()
        {
            TestDb tdb = new TestDb();
            ApplicationDbContext db = tdb.Db;

            tdb.CreateChartFields(1);
            db.SaveChanges();

            Assert.Single(db.Accounts.ToList<Account>());
            Assert.Single(db.BusinessUnits.ToList<BusinessUnit>());
            Assert.Single(db.Classes.ToList<Class>());
            Assert.Single(db.DeptIDs.ToList<DeptID>());
            Assert.Single(db.Funds.ToList<Fund>());
            Assert.Single(db.Programs.ToList<Models.ChartFields.Program>());
            Assert.Single(db.Projects.ToList<Project>());
            Assert.Single(db.Sponsors.ToList<Sponsor>());
        }

        [Fact]
        public void CreateChartStringTest()
        {
            TestDb tdb = new TestDb();
            ApplicationDbContext db = tdb.Db;

            tdb.CreateChartFields(10);
            db.SaveChanges();

            tdb.CreateChartStrings(10);
            db.SaveChanges();

            Assert.Equal(10, db.ChartStrings.Count());

            ChartString cs = db.ChartStrings.Where(st => st.Id == 2).FirstOrDefault();
            Assert.NotNull(cs);

            Assert.NotNull(cs.GetChartField<Account>());
            Assert.NotNull(cs.GetChartField<BusinessUnit>());
            Assert.NotNull(cs.GetChartField<Class>());
            Assert.NotNull(cs.GetChartField<DeptID>());
            Assert.NotNull(cs.GetChartField<Fund>());
            Assert.NotNull(cs.GetChartField<Models.ChartFields.Program>());
            Assert.NotNull(cs.GetChartField<Project>());
            Assert.NotNull(cs.GetChartField<Sponsor>());
        }

        [Fact]
        public void TestCorrectFieldOrderInChartString()
        {
            TestDb tdb = new TestDb();
            ApplicationDbContext db = tdb.Db;

            tdb.CreateChartFields(1);
            db.SaveChanges();
            tdb.CreateChartStrings(1);
            db.SaveChanges();

            ChartString cs = db.ChartStrings.FirstOrDefault();
            string[] fieldStrings = cs.GetString(":").Split(':');
            int i = 0;
            Assert.Equal(db.BusinessUnits.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Accounts.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Funds.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.DeptIDs.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Programs.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Classes.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Projects.FirstOrDefault().Value, fieldStrings[i++]);
            Assert.Equal(db.Sponsors.FirstOrDefault().Value, fieldStrings[i++]);
        }
    }
}
