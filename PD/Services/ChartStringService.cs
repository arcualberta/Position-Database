using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Models;
using PD.Models.ChartFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    /// <summary>
    /// ChartStringService handles manipulation of chart fields and chart strings
    /// </summary>
    public class ChartStringService : PdServiceBase
    {
        public ChartStringService(ApplicationDbContext db, IPdDataProtector dataProtector)
            : base(db, dataProtector)
        {
        }
        /// <summary>
        /// Creates a ChartField of a given type and adds it to the database if requested.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T CreateChartField<T>(string value) where T:ChartField, new()
        {
            T field = new T() { Value = value };
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
        public ChartString CreateChartString(
            BusinessUnit busUnit, 
            Account acc,
            Fund fund,
            DeptID deptId,
            Program prog,
            Class cls, 
            Project proj, 
            Sponsor spon)
        {
            ChartString cs = new ChartString();

            cs.SetChartField<BusinessUnit>(busUnit);
            cs.SetChartField<Account>(acc);
            cs.SetChartField<Fund>(fund);
            cs.SetChartField<DeptID>(deptId);
            cs.SetChartField<Program>(prog);
            cs.SetChartField<Class>(cls);
            cs.SetChartField<Project>(proj);
            cs.SetChartField<Sponsor>(spon);

            Db.ChartStrings.Add(cs);
            return cs;
        }




    }
}
