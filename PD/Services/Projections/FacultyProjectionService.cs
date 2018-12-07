using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            //Creating data structures to be able to calculate the projection
            if (!InitializeProjection(pa, targetDate, cycleStartDate, cycleEndDate, defaultMeritDecision))
                return null; ;

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

            //Retrieve all adjustment instances which are part of the base salary for the period covering the target date.
            //We are not creating any projected instances for these types of compensations because they are mannually assigned 
            //rather than carrying over automatically when they expire.
            List<Adjustment> adjustments = pa.GetAdjustments(targetDate, true).ToList();
                
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

            if (merit.IsPromoted)
            {

            }
            else
            {

            }

            decimal atb = Math.Round((previousSalary.Value * scale.ContractSettlement) / 100m);
            merit.Increment = Math.Round(merit.Value * scale.StepValue);
            targetSalary.Value = previousSalary.Value + atb + merit.Increment;

            foreach (var adjustmet in adjustments)
                targetSalary.Value += adjustmet.Value;

            Faculty faculty = pa.Position as Faculty;

            if (targetSalary.Value > scale.Maximum)
            {
                //If the individual is not prompted, then set the salary to the maximum
                if(pa.Position is Faculty)
                {
                    
                    //faculty.Rank == Faculty.eRank.
                }
                if (!merit.IsPromoted)
                    targetSalary.Value = scale.Maximum;
                else
                    Promote(pa, previousSalary.EndDate, targetSalary.StartDate);
            }
        }

        public PositionAssignment Promote(PositionAssignment pa, DateTime currentCycleEndDate, DateTime nextCycleEndDate, string newPositionTitle = null)
        {
            return null;

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


        public void ProjectSalaries(DateTime cycleStartDate, DateTime cycleEndDate)
        {
            DateTime targetDate = cycleStartDate.AddDays(1);
            decimal defaultMeritDecision = 1;
            bool updateDatabase = true;

            List<PositionAssignment> facultyPositions = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa=>pa.Person)
                .Include(pa => pa.Compensations)
                .Where(pa => pa.Position is Faculty && pa.StartDate <= targetDate && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate))
                .ToList();

            foreach(PositionAssignment pa in facultyPositions)
            {
                try
                {
                    PositionAssignment updated = ProjectSalary(pa, targetDate, cycleStartDate, cycleEndDate, defaultMeritDecision, updateDatabase);
                }
                catch(Exception ex)
                {

                }
            }

        }


    }
}
