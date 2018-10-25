using Microsoft.AspNetCore.DataProtection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PD.Services
{
    public class DataService : PdServiceBase
    {
        public decimal Salary { get; private set; }

        public DataService(ApplicationDbContext db)
            :base(db)
        {
        }

        public void ComputeFacultySalary(int positionAssignmentId, string baseYear, string nextYear)
        {
            decimal salery = -1;

            var positionAssignment = Db.PositionAssignments
                .Include(p => p.Compensations)
                .Include(p => p.Position)
                .Include(p => p.Position.Person)
                .Where(p => p.Id == positionAssignmentId)
                .FirstOrDefault();

            ////            var baseYearCompensation = positionAssignment.Compensations.Where(c => c.Year == baseYear && c is FacultyCompensation).FirstOrDefault() as FacultyCompensation;
            ////            if (baseYearCompensation == null)
            ////                throw new PdException("Unable to compute salary for " + nextYear + ". Compensation for " + baseYear + " not found.");

            ////            var newCompensation = positionAssignment.Compensations.Where(c => c.Year == nextYear && c is FacultyCompensation).FirstOrDefault() as FacultyCompensation;
            ////            if(newCompensation == null)
            ////            {
            ////                newCompensation = new FacultyCompensation();
            ////                newCompensation.Year = nextYear;
            //////                newCompensation.StartDate
            ////            }
            // decimal salary = baseYearCompensation.Salary + 

            ////string query = @"var Db = new SqlServerDb().Db;
            ////    var positionAssignment = Db.PersonPosition
            ////    .Include(p => p.Compensations)
            ////    .Include(p => p.Person)
            ////    .Include(p => p.Position)
            ////    .Where(p => p.Id == positionAssignmentId)
            ////    .FirstOrDefault();";

            ////decimal result = -1;
            ////CSharpScript.EvaluateAsync<decimal>(query,
            ////    ScriptOptions.Default.WithImports(new string[] { "PD.Data" })
            ////    .AddReferences(
            ////        Assembly.GetAssembly(typeof(ApplicationDbContext)),
            ////        Assembly.GetAssembly(typeof(SqlServerDb)))

            Salary = salery;
        }
    }
}
