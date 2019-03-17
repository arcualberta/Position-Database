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
        public ComputeContractSettlement(ApplicationDbContext db)
            : base(db, "Contract Settlement", "This rule computes contract settlement of the salary")
        {
        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
                //Previous year's salary
                Salary pastSalary = pa.GetCompensation<Salary>(targetDate.AddYears(-1), PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
            if (pastSalary == null)
                throw new Exception(string.Format("Past year's salary not found for the target date of {0}", targetDate));

            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
            if (scale == null)
                throw new Exception(string.Format("Salary scale not found for the year of {0}", targetDate));

            pa.LogInfo("Computing contract settlement.", pa.GetCycleYearRange(targetDate));
            ContractSettlement atb = pa.Compensations
                .Where(c => c is ContractSettlement
                    && c.StartDate <= targetDate
                    && (c.EndDate.HasValue == false || c.EndDate > targetDate))
                .FirstOrDefault() as ContractSettlement;

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
