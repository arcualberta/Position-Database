using PD.Data;
using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public abstract class ProjectionService: PdServiceBase
    {
        public ProjectionService(ApplicationDbContext db)
            : base(db)
        {
        }

        public abstract bool UpdateSalary(int positionId, string targetYear, decimal defaultMeritDecision);

    }
}
