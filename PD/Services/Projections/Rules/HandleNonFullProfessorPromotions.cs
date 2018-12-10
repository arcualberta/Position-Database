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

                //We are here because the individual received a promotion
                PromotionScheme scheme = Db.PromotionSchemes.Where(sc => sc.CurrentTitle == pa.Position.Title).FirstOrDefault();
                if (scheme == null)
                    throw new Exception(string.Format("Promotion scheme for {0} not found", pa.Position.Title));

                //Putting an end date for the current position assignment and it's associated position
                PositionAssignment oldPositionAssignment = pa;
                oldPositionAssignment.EndDate = pa.GetCycleStartDate(targetDate).AddYears(1).AddDays(-1);
                oldPositionAssignment.Position.EndDate = oldPositionAssignment.EndDate;

                //Creating a new position assignment
                pa = new PositionAssignment()
                {
                    StartDate = oldPositionAssignment.EndDate.Value.AddDays(1),
                    PersonId = oldPositionAssignment.PersonId,
                    SalaryCycleStartDay = oldPositionAssignment.SalaryCycleStartDay,
                    SalaryCycleStartMonth = oldPositionAssignment.SalaryCycleStartMonth,
                    PositionId = oldPositionAssignment.PositionId,
                    Status = oldPositionAssignment.Status,
                };

                Position position = new Faculty()
                {
                    ContractType = oldPositionAssignment.Position.ContractType,
                    Number = oldPositionAssignment.Position.Number,
                    Rank = Enum.Parse<Faculty.eRank>(scheme.PromotedTitle),
                    StartDate = pa.StartDate,
                    Title = scheme.PromotedTitle,
                    Workload = oldPositionAssignment.Position.Workload,
                };
                position.PositionAssignments.Add(pa);
                pa.Position = position;

                //Carrying over the same position accounts
                List<PositionAccount> accounts = Db.PositionAccounts.Where(pacc => pacc.PositionId == oldPositionAssignment.PositionId).ToList();
                foreach (var acc in accounts)
                    position.PositionAccounts.Add(acc);

                //TODO: 
                // Get the previous salary from the old account
                // Get the merit from the previous position and recompute it based on the scale of the new position
                // Get the ATB from the past position
                // Get all adjustments for the target year from the past position 
                // Recalculate the new aggregated salary

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
