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

        public override bool Execute(PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                //Past year's salary
                Salary pastSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1));
                if (pastSalary == null)
                    throw new Exception(string.Format("Past year's salary not found for the target date of {0}", targetDate));

                Merit merit = pa.GetCompensation<Merit>(targetDate);
                if (merit == null)
                    throw new Exception(string.Format("Merit not found for the year of {0}", targetDate));

                ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate);
                if (atb == null)
                    throw new Exception(string.Format("Contract Settlement not found for the year of {0}", targetDate));

                List<Adjustment> adjustments = pa.GetAdjustments(targetDate, true).ToList();

                Salary salary = pa.GetCompensation<Salary>(targetDate);
                if (salary == null)
                    throw new Exception(string.Format("Salary not found for the year of {0}", targetDate));

                salary.Value = 



                return true;

            }
            catch (Exception ex)
            {
                pa.LogError(ex.Message);
                return false;
            }
        }
    }
}
