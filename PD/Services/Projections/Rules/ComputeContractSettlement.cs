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

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                //Past year's salary
                Salary pastSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1));
                if (pastSalary == null)
                    throw new Exception(string.Format("Past year's salary not found for the target date of {0}", targetDate));

                SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
                if (scale == null)
                    throw new Exception(string.Format("Salary scale not found for the year of {0}", targetDate));

                ContractSettlement atb = pa.GetCompensation<ContractSettlement>(targetDate);
                if (atb == null)
                {
                    DateTime startDate = pa.GetCycleStartDate(targetDate);
                    atb = new ContractSettlement()
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddYears(1).AddDays(-1),
                        IsProjection = true,
                        Notes = string.Format("Projected on {0}", DateTime.Now)
                    };
                    pa.Compensations.Add(atb);
                }
                atb.Value = Math.Round((scale.ContractSettlement * pastSalary.Value) / 100m);
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
