using System;
using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Models.ChartFields;
using PD.Services;
using System.Linq;

namespace PD.Test.Db
{
    public class InMemoryDb
    {
        public ApplicationDbContext Db
        {
            get
            {
                if (mDb == null)
                {
                    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
                    mDb = new ApplicationDbContext(options);
                }
                return mDb;
            }
        }
        private ApplicationDbContext mDb;

        private Random mRand = new Random();
        public string GetRandNumString(int length)
        {
            string str = "";
            for (int i = 0; i < length; ++i)
                str = str + mRand.Next(10).ToString();
            return str;
        }
        public void CreateChartFields(int numFieldsPerType)
        {
            ChartStringService csrv = new ChartStringService(Db);

            for (int i = 0, n = 0; n < numFieldsPerType; ++n)
            {
                csrv.CreateChartField<Account>("1" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<BusinessUnit>("2" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Class>("3" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<DeptID>("4" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Fund>("5" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Program>("6" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Project>("7" + GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Sponsor>("8" + GetRandNumString(4)).Id = ++i;
            }
            Db.SaveChanges();
        }

        public void CreateChartStrings(int numChartStrings)
        {
            ChartStringService csrv = new ChartStringService(Db);
            var accounts = Db.Accounts.ToList<Account>();
            var businessUnits = Db.BusinessUnits.ToList<BusinessUnit>();
            var classes = Db.Classes.ToList<Class>();
            var deptIds = Db.DeptIDs.ToList<DeptID>();
            var funds = Db.Funds.ToList<Fund>();
            var programs = Db.Programs.ToList<Program>();
            var projects = Db.Projects.ToList<Project>();
            var sponsors = Db.Sponsors.ToList<Sponsor>();

            for (int i = 0; i < numChartStrings; ++i)
            {
                csrv.CreateChartString(
                    businessUnits[mRand.Next(businessUnits.Count)],
                    accounts[mRand.Next(accounts.Count)],
                    funds[mRand.Next(funds.Count)],
                    deptIds[mRand.Next(deptIds.Count)],
                    programs[mRand.Next(programs.Count)],
                    classes[mRand.Next(classes.Count)],
                    projects[mRand.Next(projects.Count)],
                    sponsors[mRand.Next(sponsors.Count)]
                    ).Id = i + 1;
            }
        }

        public void InitTestData(int numChartFields = 100, int numChartStrings = 10)
        {
            ChartStringService csrv = new ChartStringService(Db);

            //creating chart fields
            for (int i = 0; i < numChartFields; )
            {
                csrv.CreateChartField<Account>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<BusinessUnit>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Class>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<DeptID>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Fund>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Program>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Project>(GetRandNumString(4)).Id = ++i;
                csrv.CreateChartField<Sponsor>(GetRandNumString(4)).Id = ++i;
            }

            Db.SaveChanges();

            //creating chart strings
            var accounts = Db.Accounts.ToListAsync<Account>().Result;
            var businessUnits = Db.BusinessUnits.ToListAsync<BusinessUnit>().Result;
            var classes = Db.Classes.ToListAsync<Class>().Result;
            var deptIds = Db.DeptIDs.ToListAsync<DeptID>().Result;
            var funds = Db.Funds.ToListAsync<Fund>().Result;
            var programs = Db.Programs.ToListAsync<Program>().Result;
            var projects = Db.Projects.ToListAsync<Project>().Result;
            var sponsors = Db.Sponsors.ToListAsync<Sponsor>().Result;
            for (int i =0; i<numChartStrings; ++i)
            {
                csrv.CreateChartString(
                    businessUnits[mRand.Next(businessUnits.Count)],
                    accounts[mRand.Next(accounts.Count)],
                    funds[mRand.Next(funds.Count)],
                    deptIds[mRand.Next(deptIds.Count)],
                    programs[mRand.Next(programs.Count)],
                    classes[mRand.Next(classes.Count)],
                    projects[mRand.Next(projects.Count)],
                    sponsors[mRand.Next(sponsors.Count)]
                    ).Id = i + 1;
            }
        }
    }
}
