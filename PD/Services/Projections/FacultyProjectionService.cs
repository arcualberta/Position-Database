using PD.Data;
using PD.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections
{
    public class FacultyProjectionService: ProjectionService
    {
        public FacultyProjectionService(ApplicationDbContext db)
           : base(db)
        {
        }

        public override bool UpdateSalary(int positionId, string targetYear, decimal defaultMeritDecision)
        {
            Position position = Db.Positions.Where(pos => pos.Id == positionId).FirstOrDefault(); 

            throw new NotImplementedException();
        }
    }
}
