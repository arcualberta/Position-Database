using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;

namespace PD.Services.Projections.Rules.ContractSettlementComputations
{
    public class ComputeContractSettlement : AbstractProjectionRule
    {
        public ComputeContractSettlement(ApplicationDbContext db, IPdDataProtector dp)
            : base(db, dp, "Contract Settlement", "This rule computes contract settlement of the salary")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            //Previous year's salary
            Salary pastSalary = GetPastSalary(pa, targetDate);

            //Current year's salary scale
            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);

            pa.LogInfo("Computing contract settlement.", pa.GetCycleYearRange(targetDate));
            ContractSettlement atb = pa.GetCompensations<ContractSettlement>(targetDate).FirstOrDefault();

            //If no contract settlement exist for the specified perior, create a new one.
            //Note that contract settlements always has an end date.
            if (atb == null)
            {
                DateTime startDate = pa.GetCycleStartDate(targetDate);
                atb = new ContractSettlement()
                {
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1)
                };
                pa.Compensations.Add(atb);
            }

            //Calculate the dollar value of the contract settlement
            atb.Value = (scale.ContractSettlement * pastSalary.Value) / 100m;
            atb.Value = Math.Round(atb.Value);

            //If the salary scale is still a projection then the result of this calculation
            //is also a projection.
            atb.IsProjection = scale.IsProjection;

            pa.LogInfo("Contract Settlement: $" + atb.Value, pa.GetCycleYearRange(targetDate));
            return true;
        }
    }
}
