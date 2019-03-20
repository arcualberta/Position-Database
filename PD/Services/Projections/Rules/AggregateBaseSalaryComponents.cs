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
        public AggregateBaseSalaryComponents(ApplicationDbContext db, IPdDataProtector dp)
            : base(db, dp, "Contract Settlement", "This rule computes contract settlement of the salary")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            //Previous year's salary
            Salary pastSalary = GetPastSalary(pa, targetDate);

            pa.LogInfo("Aggregating base salary components.", pa.GetCycleYearRange(targetDate));

            IEnumerable<Compensation> compensationsInCurrentPeriod =
                pa.GetCompensations(targetDate).ToList();

            Salary currentSalary = compensationsInCurrentPeriod
                .Where(c => c is Salary)
                .Select(c => c as Salary)
                .FirstOrDefault();

            if (currentSalary == null)
            {
                DateTime startDate = pa.GetCycleStartDate(targetDate);
                currentSalary = new Salary()
                {
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1)
                };
                pa.Compensations.Add(currentSalary);
            }

            //Resetting the Calue and IsProjection flags of the current salary
            currentSalary.Value = pastSalary.Value;
            currentSalary.IsProjection = false;

            foreach (Compensation c in compensationsInCurrentPeriod)
            {
                if (c == currentSalary)
                    continue;

                //Add all values of compensations in the current period to the current salary value
                currentSalary.Value += c.Value;

                //If any compensation in the current period is a projection then set
                //the current salary also to be a projection.
                currentSalary.IsProjection |= c.IsProjection;
            }

            currentSalary.Value = Math.Round(currentSalary.Value);
            pa.LogInfo("Aggregated salary: $" + currentSalary.Value, pa.GetCycleYearRange(targetDate));
            return true;
        }
    }
}
