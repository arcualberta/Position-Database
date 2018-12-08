using PD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public abstract class AbstractProjectionRule
    {
        public abstract bool Update(PositionAssignment pa, DateTime targetDate);
    }
}
