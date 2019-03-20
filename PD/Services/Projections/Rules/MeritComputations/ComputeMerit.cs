using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules.MeritComputations
{
    public class ComputeMerit : AbstractProjectionRule
    {
        public ComputeMerit(ApplicationDbContext db, IPdDataProtector dp)
            : base(db, dp, "Compute Merit", "This rule computes the standard merit based on the merit decision, merit step and the position workload.")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            //This method of merit calculation does not apply to full professors
            if (pa.Position.Title == Faculty.eRank.Professor1.ToString() || pa.Position.Title == Faculty.eRank.Professor2.ToString() || pa.Position.Title == Faculty.eRank.Professor3.ToString())
                return false;

            //Previous year's salary
            Salary pastSalary = GetPastSalary(pa, targetDate);

            //Current year's salary scale
            SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);

            pa.LogInfo("Computing merit for none-full professors", pa.GetCycleYearRange(targetDate));

            Merit merit = pa.GetCompensations<Merit>(targetDate).FirstOrDefault();
            if (merit == null)
            {
                DateTime startDate = pa.GetCycleStartDate(targetDate);
                merit = new Merit()
                {
                    StartDate = startDate,
                    EndDate = startDate.AddYears(1).AddDays(-1),
                    MeritDecision = scale.DefaultMeritDecision,
                    IsProjection = true
                };
                pa.Compensations.Add(merit);
            }

            merit.Value = merit.MeritDecision * scale.StepValue * pa.Position.Workload;
            merit.Value = Math.Round(merit.Value);

            pa.LogInfo("Merit: $" + merit.Value, pa.GetCycleYearRange(targetDate));
            return true;
        }
    }
}
