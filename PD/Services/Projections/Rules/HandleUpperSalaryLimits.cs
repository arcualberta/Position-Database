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
        public HandleUpperSalaryLimits(ApplicationDbContext db, IPdDataProtector dp, SalaryScaleService salaryScaleService)
            : base(db, dp, salaryScaleService, "Handle upper salary limits", "")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            if (pa.Position.Title == Faculty.eRank.Professor3.ToString()
                || pa.Position.Title == Faculty.eRank.Professor2.ToString()
                || pa.Position.Title == Faculty.eRank.Professor1.ToString())
                return false; //No upper limit for full professors

            Salary salary = pa.GetSalary(targetDate);

            SalaryScale scale = _salaryScaleService.GetSalaryScale(pa.Position.Title, targetDate);

            if (salary.Value <= scale.Maximum)
                return false;

            //You are here because the salary is higher than or equal the maximum limit for the scale.

            pa.LogInfo("Handling upper salary limit", targetDate);
            salary.IsMaxed = true;

            decimal overpayment = salary.Value - scale.Maximum;

            //Rule 1: reduce the merit if, if applicable
            Merit merit = pa.GetCompensations<Merit>(targetDate).FirstOrDefault();
            if (merit != null && merit.Value > 0.01m)
            {
                pa.LogInfo("Adjusting merits to handle upper salary limit.", targetDate);
                pa.LogInfo("Original merit: $" + merit.Value, targetDate);
                if (merit.Value >= overpayment)
                {
                    //Here, we can reduce the merit and nullify the overpayment. 
                    //We also adjust the salary accordingly
                    merit.Value = merit.Value - overpayment;
                    salary.Value = salary.Value - overpayment;
                    overpayment = 0;
                }
                else
                {
                    //Here we set the merit to zero and adjust the salary accordingly but
                    //we will still have some overpayment left.
                    salary.Value = salary.Value - merit.Value;
                    overpayment = overpayment - merit.Value;
                    merit.Value = 0;

                }
                pa.LogInfo("Adjusted merit: $" + merit.Value, targetDate);
            }

            //Rule 2: if overpayment is still positive, try to reduce it by bringing down the contract settlement
            if (overpayment > 0m)
            {
                pa.LogInfo("Salary exceeds the upper limit by $" + overpayment, targetDate);

                ContractSettlement atb = pa.GetCompensations<ContractSettlement>(targetDate).FirstOrDefault();
                if (atb != null && atb.Value > 0.01m)
                {
                    pa.LogInfo("Adjusting contract settlement to handle upper salary limit.", targetDate);
                    pa.LogInfo("Original contract settlement: $" + atb.Value, targetDate);

                    if (atb.Value >= overpayment)
                    {
                        //Here, we can reduce the contract settlement and nullify the overpayment
                        //We also adjust the salary accordingly
                        atb.Value = atb.Value - overpayment;
                        salary.Value = salary.Value - overpayment;
                        overpayment = 0;
                    }
                    else
                    {
                        //Here we set the contract settlement to zero and adjust the salary accordingly.
                        //However, we may still have some overpayment left.
                        salary.Value = salary.Value - atb.Value;
                        overpayment = overpayment - atb.Value;
                        atb.Value = 0;
                    }
                    pa.LogInfo("Adjusted contract settlement: $" + atb.Value, targetDate);
                }
            }

            //If we still have excess, then we will report it as a warning.
            if (overpayment > 0m)
                pa.LogWarning("Overpaid beyond the max salary limit for the scale!", targetDate);

            return true;
        }
    }
}
