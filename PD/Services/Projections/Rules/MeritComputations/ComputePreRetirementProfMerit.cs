using PD.Data;
using PD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules.MeritComputations
{
    public class ComputePreRetirementProfMerit : AbstractProjectionRule
    {
        public ComputePreRetirementProfMerit(ApplicationDbContext db, IPdDataProtector dp, SalaryScaleService salaryScaleService)
            : base(db, dp, salaryScaleService, "Compute Pre-retirement Professor Merit", "")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            if (!pa.IsPaidOn(targetDate))
                return true;

            throw new NotImplementedException("ComputePostRetirementProfMerit.Execute not implemented yet.");
        }
    }
}
