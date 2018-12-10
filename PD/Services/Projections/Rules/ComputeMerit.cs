using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public class ComputeMerit : AbstractProjectionRule
    {
        public ComputeMerit(ApplicationDbContext db)
            : base(db, "Compute Merit", "This rule computes the standard merit based on the merit decision, merit step and the position workload.")
        {

        }

        public override bool Execute(PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
                if (scale == null)
                    throw new Exception(string.Format("Salary scale not found for the year of {0}", targetDate));

                Merit merit = pa.GetCompensation<Merit>(targetDate);
                if (merit == null)
                {
                    DateTime startDate = pa.GetCycleStartDate(targetDate);
                    merit = new Merit()
                    {
                        StartDate = startDate,
                        EndDate = startDate.AddYears(1).AddDays(-1),
                        MeritDecision = scale.DefaultMeritDecision,
                        IsProjection = true,
                        Notes = string.Format("Projected on {0}", DateTime.Now)
                    };
                    pa.Compensations.Add(merit);
                }
                merit.Value = Math.Round(merit.MeritDecision * scale.StepValue * pa.Position.Workload);
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
