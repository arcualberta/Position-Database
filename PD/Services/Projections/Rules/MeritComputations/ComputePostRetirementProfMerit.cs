using PD.Data;
using PD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules.MeritComputations
{
    public class ComputePostRetirementProfMerit : AbstractProjectionRule
    {
        public ComputePostRetirementProfMerit(ApplicationDbContext db)
            : base(db, "Compute Pre-retirement Professor Merit", "")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            throw new NotImplementedException();
        }
    }
}
