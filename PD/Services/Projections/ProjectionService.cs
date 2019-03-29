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
        public ProjectionService(ApplicationDbContext db, IPdDataProtector dataProtector)
            : base(db, dataProtector)
        {
        }

        public abstract PositionAssignment CalculateSalary(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag);

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

            return ProjectSalary(pa, targetDate, defaultMeritDecision, updateDatabase);
        }

        public virtual PositionAssignment ProjectSalary(
            PositionAssignment pa,
            DateTime targetDate,
            decimal defaultMeritDecision,
            bool updateDatabase = false)
        {
            //Creating data structures to be able to calculate the projection
            DateTime cycleStartDate = pa.GetSalaryCycleStartDate(targetDate);
            if (!InitializeProjection(pa, targetDate, cycleStartDate, cycleStartDate.AddYears(1).AddDays(-1), defaultMeritDecision))
                return null; ;

            // STEP 2: Updating the salary parameters
            // ======================================
            CalculateSalary(pa, targetDate, true);

            if (updateDatabase)
                Db.SaveChanges();

            return pa;
        }
        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            return Db.SalaryScales
                .Where(sc => sc.StartDate <= targetDate && sc.EndDate >= targetDate && sc.Category == positionTitle)
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
            Salary salary = pa.GetCompensation<Salary>(targetDate, true);
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
            Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (merit == null)
            {
                merit = new Merit()
                {
                    StartDate = cycleStartDate,
                    EndDate = cycleEndDate,
                    IsProjection = true,
                    PositionAssignmentId = pa.Id,
                    Notes = "Projected on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    MeritDecision = defaultMeritDecision,
                    Value = 0m,
                    IsPromoted = false
                };
                pa.Compensations.Add(merit);
            }

            //Retrieve the confirmed ContractSettlement for the period covering the target date.
            //If one does not exist, then try to retrieve projected one. 
            //If it doesn't exist either, then create a projected one.
            ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (atb == null)
            {
                atb = new ContractSettlement()
                {
                    StartDate = cycleStartDate,
                    EndDate = cycleEndDate,
                    IsProjection = true,
                    PositionAssignmentId = pa.Id,
                    Notes = "Projected on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                pa.Compensations.Add(atb);
            }
            
            return true;
        }

        public Salary ProjectRegularSalary(PositionAssignment pa, DateTime targetDate)
        {
            //To update the salary for the target year, we need the following prerequisites.
            //If the projection flag is set, we can work with projected values for these pre-requisites if their confirmed 
            //values do not exist.
            //============================================================================================================

            //Previous yeaar's salary. Give priority for the confirmed salary, if it exists.
            Salary previousSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1), PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (previousSalary == null)
                throw new Exception("Previous year's salary not found.");

            //Adjustment instances which are part of the base salary for the period covering the target date.
            List<Adjustment> adjustments = pa.GetAdjustments(targetDate, true).ToList();

            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
            if (scale == null)
                throw new Exception("Salary scale not found for the target interval of " + targetDate);

            //Here we make sure to retrieve the projection salary instance, not a confirmed instance, since we do not want to
            //overwrite confirmed values with projections
            Salary newSalary = pa.GetCompensation<Salary>(targetDate, true);
            if (newSalary == null)
                throw new Exception("Required salary object to be updated not loaded.");

            //Target year's merit. Give priority to confirmed merit first.
            Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (merit == null)
                throw new Exception("Merit details not found for the target interval of " + targetDate);

            ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (atb == null)
                throw new Exception("Contract Settlement details not found for the target interval of " + targetDate);

            //Assign calculated ATB value
            atb.Value = Math.Round((previousSalary.Value * scale.ContractSettlement) / 100m);

            if (merit.Value < 0.0001m) //Merit of zero
            {
                merit.Value = 0m;

                //Check if the individual was already in the top of the scale last year
                SalaryScale prevYearScale = GetSalaryScale(pa.Position.Title, targetDate.AddYears(-1));
                if(prevYearScale == null)
                    throw new Exception("Salary scale not found for the PREVIOUS YEAR of the target interval containing " + targetDate);

                if(Math.Abs(previousSalary.Value - prevYearScale.Maximum) < 1m)
                {
                    //This individual was at the top of the scale in the previous year. Therefore, we need to add whatever the amount 
                    //to the new salary the Contract Settlement to keep him/her at the top of the scale this year too.

                    //However, before doing so, we make sure the difference between previous year's salary + this year's atb and this year's
                    //max salary is small enough before assigning this year's max salary to the individual. It's done this way to
                    //curb the risk of giving unwanted max salary in case additional steps are added to the current year's salary scale due to
                    //whatever reason.
                    if (Math.Abs(previousSalary.Value + atb.Value - scale.Maximum) < 10)
                    {
                        newSalary.Value = scale.Maximum;
                        atb.Value = newSalary.Value - previousSalary.Value;
                    }
                    else
                        newSalary.Value = previousSalary.Value + atb.Value;
                }
                else
                    newSalary.Value = previousSalary.Value + atb.Value;
            }
            else
            {
                //First add the contract settlement to the new salary
                decimal salary_before_merit = previousSalary.Value + atb.Value;

                //Now calculate the merit increment based on the given merit decision and the step value
                merit.Value = Math.Round(merit.MeritDecision * scale.StepValue);

                //If the new salary exceeds the max for the scale when the increment is added, OR
                //if the new salary becomes very close to the max salary but smaller (say, within $10) when the increment is added
                //THEN we adjust the increment such that the resultant salary becomes the max salary when the increment is applied.
                decimal tentative_new_salary = salary_before_merit + merit.Value;
                if (tentative_new_salary > scale.Maximum || (scale.Maximum - tentative_new_salary) < 10m)
                    merit.Value = scale.Maximum - salary_before_merit;

                //Now we finally add the increment to the final salary.
                newSalary.Value = salary_before_merit + merit.Value;
            }


            foreach (var adjustment in adjustments)
                newSalary.Value += adjustment.Value;

            if (newSalary.Value > scale.Maximum)
            {
                newSalary.Value = scale.Maximum;
                newSalary.IsMaxed = true;
            }

            return newSalary;
        }

    }
}
