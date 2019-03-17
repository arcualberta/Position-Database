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
        public ComputePreRetirementProfMerit(ApplicationDbContext db, IPdDataProtector dp)
            : base(db, dp, "Compute Pre-retirement Professor Merit", "")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            throw new NotImplementedException();
        }
    }
}
