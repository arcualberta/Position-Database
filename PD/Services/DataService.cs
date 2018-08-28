using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public class DataService : PdServiceBase
    {
        public DataService(ApplicationDbContext db)
            :base(db)
        {

        }

        public decimal ComputeFacultySalary(int positionAssignmentId, string baseYear, string nextYear)
        {
            decimal salery = -1;

            var positionAssignment = Db.PersonPositions
                .Include(p => p.Compensations)
                .Include(p => p.Person)
                .Include(p => p.Position)
                .Where(p => p.Id == positionAssignmentId)
                .FirstOrDefault();

            var baseYearCompensation = positionAssignment.Compensations.Where(c => c.Year == baseYear && c is FacultyCompensation).FirstOrDefault() as FacultyCompensation;
            if (baseYearCompensation == null)
                throw new PdException("Unable to compute salary for " + nextYear + ". Compensation for " + baseYear + " not found.");

            var newCompensation = positionAssignment.Compensations.Where(c => c.Year == nextYear && c is FacultyCompensation).FirstOrDefault() as FacultyCompensation;
            if(newCompensation == null)
            {
                newCompensation = new FacultyCompensation();
                newCompensation.Year = nextYear;
//                newCompensation.StartDate
            }
           // decimal salary = baseYearCompensation.Salary + 

            return salery;
        }
    }
}
