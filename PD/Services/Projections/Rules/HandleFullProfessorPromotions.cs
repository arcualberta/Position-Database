using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;

namespace PD.Services.Projections.Rules
{
    public class HandleFullProfessorPromotions : AbstractProjectionRule
    {
        public HandleFullProfessorPromotions(ApplicationDbContext db)
            : base(db, "Handle full professor promotions", "")
        {

        }

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                if (!(pa.Position.Title == Faculty.eRank.Professor1.ToString() || pa.Position.Title == Faculty.eRank.Professor2.ToString() || pa.Position.Title == Faculty.eRank.Professor3.ToString()))
                    return false;

                Salary salary = pa.GetCompensation<Salary>(targetDate, true);
                if (salary == null)
                    throw new Exception(string.Format("Projected salary not found for the year of {0}", targetDate));

                if (salary.Value <= 0.01m)
                    throw new Exception(string.Format("Handle full prof promotion: salary is not computed for the year of {0}", targetDate));

                SalaryScale scale = GetSalaryScale(pa.Position.Title, targetDate);
                if (scale == null)
                    throw new Exception(string.Format("Salary scale not found for the year of {0}", targetDate));

                if (salary.Value <= scale.Maximum)
                    return false;

                pa.LogInfo("Handling full professor promotion");

                Merit merit = pa.GetCompensation<Merit>(targetDate);
                if (merit == null)
                    throw new Exception(string.Format("Merit not found for the year of {0}", targetDate));

                throw new Exception("Full prof promotion handling not yet implemented");



                return true;

            }
            catch(Exception ex)
            {
                pa.LogError(ex.Message);
                return false;
            }
        }
    }
}
