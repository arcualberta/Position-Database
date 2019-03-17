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
        public AggregateBaseSalaryComponents(ApplicationDbContext db)
            : base(db, "Contract Settlement", "This rule computes contract settlement of the salary")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                //Past year's salary
                Salary pastSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1), PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (pastSalary == null)
                    throw new Exception(string.Format("Past year's salary not found for the target date of {0}", targetDate));

                Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (merit == null)
                    throw new Exception(string.Format("Merit not found for the year of {0}", targetDate));

                ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (atb == null)
                    throw new Exception(string.Format("Contract Settlement not found for the year of {0}", targetDate));

                List<Adjustment> adjustments = pa.GetAdjustments(targetDate, true).ToList();

                pa.LogInfo("Aggregating base salary components.", pa.GetCycleYearRange(targetDate));

                Salary salary = pa.GetCompensation<Salary>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);

                if (salary == null)
                {
                    DateTime startDate = pa.GetCycleStartDate(targetDate);
                    salary = new Salary()
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddYears(1).AddDays(-1)
                    };
                    pa.Compensations.Add(salary);
                }

                salary.Value = pastSalary.Value + merit.Value + atb.Value;
                salary.IsProjection = pastSalary.IsProjection || merit.IsProjection || atb.IsProjection;

                foreach (Adjustment adj in adjustments)
                {
                    salary.Value += adj.Value;
                    salary.IsProjection |= adj.IsProjection;
                }

                salary.Value = Math.Round(salary.Value);
                pa.LogInfo("Aggregated salary: $" + salary.Value, pa.GetCycleYearRange(targetDate));
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
