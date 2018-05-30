using PAD.Data;
using PAD.Models;
using PAD.Models.ChartFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAD.Services
{
    /// <summary>
    /// ChartStringService handles manipulation of chart fields and chart strings
    /// </summary>
    public class ChartStringService : PadServiceBase
    {
        public ChartStringService(ApplicationDbContext db)
            : base(db)
        {

        }
        /// <summary>
        /// Creates a ChartField of a givent type and adds it to the database if requested.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T CreateChartField<T>(string value, bool addToDb = true) where T:ChartField, new()
        {
            T field = new T() { Value = value };
            if (addToDb)
                Db.ChartFields.Add(field);
            return field;
        }

        /// <summary>
        /// Creates a chart string and adds it to the database if requested.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="busUnit"></param>
        /// <param name="cls"></param>
        /// <param name="deptId"></param>
        /// <param name="fund"></param>
        /// <param name="prog"></param>
        /// <param name="proj"></param>
        /// <param name="spon"></param>
        /// <param name="addToDb"></param>
        /// <returns></returns>
        public ChartString CreateChartString(Account acc, BusinessUnit busUnit, 
            Class cls, DeptID deptId, Fund fund, Program prog,
            Project proj, Sponsor spon, bool addToDb = true)
        {
            ChartString cs = new ChartString();

            cs.SetChartField<Account>(acc);
            cs.SetChartField<BusinessUnit>(busUnit);
            cs.SetChartField<Class>(cls);
            cs.SetChartField<DeptID>(deptId);
            cs.SetChartField<Fund>(fund);
            cs.SetChartField<Program>(prog);
            cs.SetChartField<Project>(proj);
            cs.SetChartField<Sponsor>(spon);

            if (addToDb)
                Db.ChartStrings.Add(cs);
            return cs;
        }




    }
}
