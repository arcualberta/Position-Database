using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public class AggregateBaseSalaryComponents : AbstractProjectionRule
    {
        public AggregateBaseSalaryComponents(ApplicationDbContext db, IPdDataProtector dp, SalaryScaleService salaryScaleService)
            : base(db, dp, salaryScaleService, "Aggregate Base Salary Components", "This rule computes the base salary by aggregating appropriate compensation componenets.")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            //Previous year's salary
            Salary pastSalary = pa.GetPastSalary(targetDate);

            pa.LogInfo("Aggregating base salary components.", targetDate);

            IEnumerable<Compensation> compensationsInCurrentPeriod =
                pa.GetCompensations(targetDate).ToList();

            Salary currentSalary = compensationsInCurrentPeriod
                .Where(c => c is Salary)
                .Select(c => c as Salary)
                .FirstOrDefault();

            if (currentSalary == null)
            {
                DateTime startDate = pa.GetSalaryCycleStartDate(targetDate);
                currentSalary = new Salary()
                {
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1)
                };
                pa.Compensations.Add(currentSalary);
            }

            //Resetting the Value and IsProjection flags of the current salary
            currentSalary.Value = pastSalary.Value;
            currentSalary.IsProjection = false;

            foreach (Compensation c in compensationsInCurrentPeriod)
            {
                if (c == currentSalary)
                    continue;

                //If c is a base salary component then add it's value to the new salary
                if (c is Merit || c is ContractSettlement ||
                    (c is Adjustment) && (c as Adjustment).IsBaseSalaryComponent
                   )
                {
                    currentSalary.Value += c.Value;

                    //If any compensation in the current period is a projection then set
                    //the current salary also to be a projection.
                    currentSalary.IsProjection |= c.IsProjection;
                }
            }

            currentSalary.Value = Math.Round(currentSalary.Value);
            pa.LogInfo("Aggregated salary: $" + currentSalary.Value, targetDate);
            return true;
        }
    }
}
