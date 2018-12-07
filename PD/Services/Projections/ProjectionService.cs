using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public abstract class ProjectionService : PdServiceBase
    {
        public ProjectionService(ApplicationDbContext db)
            : base(db)
        {
        }

        public abstract PositionAssignment ProjectSalary(
            PositionAssignment pa,
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision,
            bool updateDatabase = false);

        public abstract void CalculateSalary(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag);

        public virtual PositionAssignment ProjectSalary(
            int positionAssignmentId,
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision,
            decimal defaultContractSettlement,
            bool updateDatabase = false)
        {
            PositionAssignment pa = GetPositionAssignment(positionAssignmentId, targetDate, true, false, true, false);
            if (pa == null)
                return null;

            return ProjectSalary(pa, targetDate, cycleStartDate, cycleEndDate, defaultMeritDecision, updateDatabase);
        }

        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            return Db.SalaryScales
                .Where(sc => sc.StartDate <= targetDate && sc.EndDate >= targetDate && sc.Name == positionTitle)
                .FirstOrDefault();
        }


        public bool InitializeProjection(
            PositionAssignment pa,
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision)
        {
            //If the position is not yet started or has been ended by the target date then no salary projection is possible.
            if (pa.StartDate.HasValue == false || pa.StartDate > targetDate || pa.EndDate < targetDate)
                return false;

            //Prerequisit: Salary scale for the target year is needed
            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
            if (scale == null)
                throw new Exception("Salary scale not found for the target interval of " + targetDate);


            // STEP 1: Retrieving or creating necessary parameters
            // ===================================================

            //Retrieve or create the salary-projection instance for the period covering the target date
            //We do not retrieve non-projection (or confirmed) salary instances for the target period because
            //we do not want to overwrite confirmed values with projections.
            Salary salary = pa.Compensations
                .Where(c => c is Salary && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection)
                .FirstOrDefault() as Salary;
            if (salary == null)
            {
                salary = new Salary()
                {
                    StartDate = cycleStartDate,
                    EndDate = cycleEndDate,
                    IsProjection = true,
                    PositionAssignmentId = pa.Id,
                    Notes = "Projected on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                pa.Compensations.Add(salary);
            }

            //Retrieve the confirmed merit for the period covering the target date.
            //If one does not exist, then try to retrieve projected one. 
            //If it doesn't exist either, then create a projected one.
            Merit merit = pa.Compensations
                .Where(c => c is Merit && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == false)
                .FirstOrDefault() as Merit;
            if (merit == null)
            {
                merit = pa.Compensations
                .Where(c => c is Merit && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == true)
                .FirstOrDefault() as Merit;
                if (merit == null)
                {
                    merit = new Merit()
                    {
                        StartDate = cycleStartDate,
                        EndDate = cycleEndDate,
                        IsProjection = true,
                        PositionAssignmentId = pa.Id,
                        Notes = "Projected on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Value = defaultMeritDecision,
                        IsPromoted = false,
                    };
                    pa.Compensations.Add(merit);
                }
            }

            return true;
        }
    }
}
