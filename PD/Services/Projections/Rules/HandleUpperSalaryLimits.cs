using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public class HandleUpperSalaryLimits : AbstractProjectionRule
    {
        public HandleUpperSalaryLimits(ApplicationDbContext db, IPdDataProtector dp)
            : base(db, dp, "Handle upper salary limits", "")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            if (pa.Position.Title == Faculty.eRank.Professor3.ToString())
                return false; //No upper limit for full professor 3

            Salary salary = GetPastSalary(pa, targetDate);

            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);

            if (salary.Value <= scale.Maximum)
                return false;

            //You are here because the salary is higher than or equal the maximum limit for the scale.

            pa.LogInfo("Handling upper salary limit", pa.GetCycleYearRange(targetDate));
            salary.IsMaxed = true;

            decimal excess = salary.Value - scale.Maximum;

            //Rule 1: reduce the merit if, if applicable
            Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (merit != null && merit.Value > 0.01m)
            {
                pa.LogInfo("Adjusting merits to handle upper salary limit.", pa.GetCycleYearRange(targetDate));
                pa.LogInfo("Original merit: $" + merit.Value, pa.GetCycleYearRange(targetDate));
                if (merit.Value >= excess)
                {
                    merit.Value = merit.Value - excess;
                    excess = 0;
                }
                else
                {
                    excess = excess - merit.Value;
                    merit.Value = 0;
                }
                pa.LogInfo("Adjusted merit: $" + merit.Value, pa.GetCycleYearRange(targetDate));
            }

            //Rule 2: if excess is still positive, try to reduce it by bringing down atb
            if (excess > 0m)
            {
                pa.LogInfo("Salary exceeds the upper limit by $" + excess, pa.GetCycleYearRange(targetDate));

                ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (atb != null && atb.Value > 0.01m)
                {
                    pa.LogInfo("Adjusting contract settlement to handle upper salary limit.", pa.GetCycleYearRange(targetDate));
                    pa.LogInfo("Original contract settlement: $" + atb.Value, pa.GetCycleYearRange(targetDate));

                    if (atb.Value >= excess)
                    {
                        atb.Value = atb.Value - excess;
                        excess = 0;
                    }
                    else
                    {
                        excess = excess - atb.Value;
                        atb.Value = 0;
                    }
                    pa.LogInfo("Adjusted contract settlement: $" + atb.Value, pa.GetCycleYearRange(targetDate));
                }
            }

            //If we still have excess, then we will report it as a warning.
            if (excess > 0m)
                pa.LogWarning("Overpaid beyond the max salary limit for the scale!", pa.GetCycleYearRange(targetDate));

            return true;
        }
    }
}
