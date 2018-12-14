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

        public override bool Execute(ref PositionAssignment pa, DateTime targetDate)
        {
            try
            {
                if (!(pa.Position.Title == Faculty.eRank.AssistantProfessor.ToString() || pa.Position.Title == Faculty.eRank.AssociateProfessor.ToString()))
                    return false;

                Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);
                if (merit == null)
                    throw new Exception(string.Format("Merit not found for the year of {0}", targetDate));

                if (!merit.IsPromoted)
                    return false;

                pa.LogInfo("Handling promotions for none-full professors");

                //We are here because the individual received a promotion
                string title = pa.Position.Title;
                PromotionScheme scheme = Db.PromotionSchemes.Where(sc => sc.CurrentTitle == title).FirstOrDefault();
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

                //Linking the previous position assignment as the predecessor for this position assignment
                pa.Predecessor = oldPositionAssignment.Predecessor;

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

                //Transferring all compensations recorded for the target year in the old position account 
                //into the new one
                List<Compensation> compensations = oldPositionAssignment.Compensations
                    .Where(c => c.StartDate <= targetDate && c.EndDate >= targetDate)
                    .ToList();
                foreach(Compensation c in compensations)
                {
                    //If the compensation start date is earlier than the start date of the new position assignment
                    //then we split the compensation into two and set the end date of the first half to be a day prior
                    //to the start date of the new position assignment and then we leave this first half with the 
                    //old position assigment, and then we carry over the second half to the new position assignment.
                    //If the start date of this compensation is as same as the start date of the new position assignment,
                    //then we simply carry it over to the new position assignment as a whole

                    if (c.StartDate < pa.StartDate)
                    {
                        c.EndDate = pa.StartDate.Value.AddDays(-1);
                        Compensation clone = c.Clone();
                        clone.StartDate = pa.StartDate.Value;
                        pa.Compensations.Add(clone);
                    }
                    else
                        pa.Compensations.Add(c);
                }

                pa.LogInfo("New position created");

                //Recalculating the merit, atb and the aggregated base salary
                new ComputeMerit(Db).Execute(ref pa, targetDate);
                new ComputeContractSettlement(Db).Execute(ref pa, targetDate);
                new AggregateBaseSalaryComponents(Db).Execute(ref pa, targetDate);

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
