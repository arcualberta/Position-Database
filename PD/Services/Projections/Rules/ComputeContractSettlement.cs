using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;

namespace PD.Services.Projections.Rules
{
    public class ComputeContractSettlement : AbstractProjectionRule
    {
        public ComputeContractSettlement(ApplicationDbContext db)
            : base(db, "Contract Settlement", "This rule computes contract settlement of the salary")
        {
        }

        public override bool Execute(PositionAssignment pa, DateTime targetDate)
        {
            //Past year's salary
            Salary pastSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1));
            if(pastSalary == null)
            {
                pa.LogError(string.Format("Past year's salary not found for the target date of {0}", targetDate));
                return false;
            }

            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
            if(scale == null)
            {
                pa.LogError(string.Format("Salary scale not found for the year of {0}", targetDate));
                return false;
            }

            ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate);
            if(atb == null)
            {
                DateTime startDate = pa.GetCycleStartDate(targetDate);
                atb = new ContractSettlement()
                {
                    Value = (scale.ContractSettlement * pastSalary.Value)/100m,
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1),
                    IsProjection = true,
                    Notes = string.Format("Projected on {0}", DateTime.Now)
                };
                pa.Compensations.Add(atb);
            }
            return true;
        }
    }
}
