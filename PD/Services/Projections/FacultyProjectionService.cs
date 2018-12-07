using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections
{
    public class FacultyProjectionService: ProjectionService
    {
        public FacultyProjectionService(ApplicationDbContext db)
           : base(db)
        {
        }

        public override PositionAssignment ProjectSalary(
            PositionAssignment pa, 
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision,
            bool updateDatabase = false)
        {
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
            if(salary == null)
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

            //Retrieve the confirmed ATB for the period covering the target date.
            //If one does not exist, then try to retrieve projected one. 
            //If it doesn't exist either, then create a projected one.
            ContractSettlement atb = pa.Compensations
                .Where(c => c is ContractSettlement && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == false)
                .FirstOrDefault() as ContractSettlement;
            if (atb == null)
            {
                atb = pa.Compensations
                .Where(c => c is ContractSettlement && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == true)
                .FirstOrDefault() as ContractSettlement;
                if (atb == null)
                {
                    atb = new ContractSettlement()
                    {
                        StartDate = cycleStartDate,
                        EndDate = cycleEndDate,
                        IsProjection = true,
                        PositionAssignmentId = pa.Id,
                        Notes = "Projected on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Value = scale.ContractSettlement
                    };
                    pa.Compensations.Add(atb);
                }
            }


            // STEP 2: Updating the salary parameters
            // ======================================
            CalculateSalary(pa, targetDate, true);

            if (updateDatabase)
                Db.SaveChanges();

            return pa;
        }

        /// <summary>
        /// Calculates the salary of the given position assignment for the specified target year.
        /// Note that this method relies on the necessary compensation components to be already loaded on to the given "pa"
        /// </summary>
        /// <param name="pa">The PositionAssignment to be updated.</param>
        /// <param name="targetDate">The target date.</param>
        /// <param name="projectionModeFlag">If set to true, then updates the projected salary. Otherwise, updates the confirmed salary </param>
        public override void CalculateSalary(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag)
        {
            Salary salaryToBeUpdated = pa.GetCompensation<Salary>(targetDate, projectionModeFlag);
            if (salaryToBeUpdated == null)
                throw new Exception("Required salary object not loaded.");

            //To update the salary for the target year, we need the following prerequisites.
            //If the projection flag is set, we can work with projected values for these pre-requisites if their confirmed 
            //values do not exist.
            //============================================================================================================

            //Previous yeaar's salary
            Salary previousSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1), false);
            if (previousSalary == null && projectionModeFlag) 
                previousSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1), true);
            if (previousSalary == null)
                throw new Exception("Previous year's salary not found.");

            //Target year's merit
            Merit merit = pa.GetCompensation<Merit>(targetDate, false);
            if (merit == null && projectionModeFlag)
                merit = pa.GetCompensation<Merit>(targetDate, true);
            if (merit == null)
                throw new Exception("Target year's merit not found.");

            //Target year's contract settlement
            ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, false);
            if (atb == null && projectionModeFlag)
                atb = pa.GetCompensation<ContractSettlement>(targetDate, true);
            if (atb == null)
                throw new Exception("Target year's contract settlement not found.");


            //Retrieve all adjustment instances which are part of the base salary for the period covering the target date.
            //We are not creating any projected instances for these types of compensations because they are mannually assigned 
            //rather than carrying over automatically when they expire.
            List<Adjustment> adjustments = Db.Compensations
                .Where(c => c is Adjustment && c.StartDate <= targetDate && c.EndDate >= targetDate && (c as Adjustment).IsBaseSalaryComponent)
                .Select(c => c as Adjustment)
                .ToList();


            if (pa.Position == null)
                throw new Exception("Position information needs to be loaded to calcualte salary.");

            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
            if (scale == null)
                throw new Exception("Salary scale not found for the target interval of " + targetDate);

            Salary targetSalary = pa.GetCompensation<Salary>(targetDate, projectionModeFlag);
            if (targetSalary == null)
                throw new Exception("Salary place holder not found for the target interval of " + targetDate);


            //Implementing the basic caluclation based on the salary scales
            //=============================================================

            targetSalary.Value =
                previousSalary.Value +
                (previousSalary.Value * atb.Value) / 100 +
                merit.Value * scale.StepValue;

            foreach (var adjustmet in adjustments)
                targetSalary.Value += adjustmet.Value;


            if(targetSalary.Value > scale.Maximum)
            {
                //If the individual is not prompted, then set the salary to the maximum
                if (!merit.IsPromoted)
                    targetSalary.Value = scale.Maximum;
                else
                    Promote(pa, previousSalary.EndDate, targetSalary.StartDate);
            }
        }

        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            return Db.SalaryScales
                .Where(sc => sc.StartDate <= targetDate && sc.EndDate >= targetDate && sc.Name == positionTitle)
                .FirstOrDefault();
        }

        public PositionAssignment Promote(PositionAssignment pa, DateTime currentCycleEndDate, DateTime nextCycleEndDate, string newPositionTitle = null)
        {
            if (string.IsNullOrEmpty(newPositionTitle))
            {
                PromotionScheme sc = Db.PromotionSchemes.Where(s => s.CurrentTitle == pa.Position.Title).FirstOrDefault();
                if (sc == null)
                    throw new Exception("Cannot determine promoted positin for " + pa.Position.Title);

                newPositionTitle = sc.PromotedTitle;
            }

            DateTime nextCycleBeginDate = currentCycleEndDate.AddDays(1);

            //Put an end date for the current position
            Faculty oldPosition = pa.Position as Faculty;
            oldPosition.EndDate = currentCycleEndDate;

            //Put an end date for the current position assignment
            PositionAssignment oldPositionAssignment = pa;
            oldPositionAssignment.EndDate = currentCycleEndDate;

            //Creating a new position assignment
            pa = new PositionAssignment();
            pa.PersonId = oldPositionAssignment.PersonId;
            pa.StartDate = nextCycleBeginDate;
            pa.EndDate = nextCycleEndDate;
            pa.Status = PositionAssignment.eStatus.Active;

            //moving the target salary from the previous position to the new position
            //...


            return pa;
        }
    }
}
