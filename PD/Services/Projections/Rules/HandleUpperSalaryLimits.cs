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
        public HandleUpperSalaryLimits(ApplicationDbContext db)
            : base(db, "Handle upper salary limits", "")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                if (pa.Position.Title == Faculty.eRank.Professor3.ToString())
                    return false; //No upper limit for full professor 3

                Salary salary = pa.GetCompensation<Salary>(targetDate, true);
                if (salary == null)
                    throw new Exception(string.Format("Projected salary not found for the year of {0}", targetDate));

                SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
                if (scale == null)
                    throw new Exception(string.Format("Salary scale not found for the year of {0}", targetDate));

                if (salary.Value <= scale.Maximum)
                    return false;

                //You are here because the salary is higher than or equal the maximum limit for the scale.

                pa.LogInfo("Handling upper salary limit", pa.GetCycleYearRange(targetDate), true);
                salary.IsMaxed = true;

                decimal excess = salary.Value - scale.Maximum;

                //Rule 1: reduce the merit if, if applicable
                Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (merit!= null && merit.Value > 0.01m)
                {
                    pa.LogInfo("Adjusting merits to handle upper salary limit.", pa.GetCycleYearRange(targetDate), true);
                    pa.LogInfo("Original merit: $" + merit.Value, pa.GetCycleYearRange(targetDate), true);
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
                    pa.LogInfo("Adjusted merit: $" + merit.Value, pa.GetCycleYearRange(targetDate), true);
                }

                //Rule 2: if excess is still positive, try to reduce it by bringing down atb
                if (excess > 0m)
                {
                    pa.LogInfo("Salary exceeds the upper limit by $" + excess, pa.GetCycleYearRange(targetDate), true);

                    ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                    if (atb != null && atb.Value > 0.01m)
                    {
                        pa.LogInfo("Adjusting contract settlement to handle upper salary limit.", pa.GetCycleYearRange(targetDate), true);
                        pa.LogInfo("Original contract settlement: $" + atb.Value, pa.GetCycleYearRange(targetDate), true);

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
                        pa.LogInfo("Adjusted contract settlement: $" + atb.Value, pa.GetCycleYearRange(targetDate), true);
                    }
                }

                //If we still have excess, then we will report it as a warning.
                if (excess > 0m)
                    pa.LogWarning("Overpaid beyond the max salary limit for the scale!", pa.GetCycleYearRange(targetDate), true);

                return true;

            }
            catch (Exception ex)
            {
                pa.LogError(ex.Message, pa.GetCycleYearRange(targetDate), true);
                return false;
            }
        }
    }
}
