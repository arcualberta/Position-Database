using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public class HandleNonFullProfessorPromotions : AbstractProjectionRule
    {
        public HandleNonFullProfessorPromotions(ApplicationDbContext db)
            : base(db, "Handle non-full professor promotions", "")
        {

        }

        public override bool Execute(PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                if (!(pa.Position.Title == Faculty.eRank.AssistantProfessor.ToString() || pa.Position.Title == Faculty.eRank.AssociateProfessor.ToString()))
                    return false;

                Merit merit = pa.GetCompensation<Merit>(targetDate);
                if (merit == null)
                    throw new Exception(string.Format("Merit not found for the year of {0}", targetDate));

                if (!merit.IsPromoted)
                    return false;

                pa.LogInfo("Handling none-full professor promotion");


                throw new Exception("None-full prof promotion handling not yet implemented");



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
