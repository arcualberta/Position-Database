using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PD.Services.Projections.Rules;
using PD.Services.Projections.Rules.MeritComputations;
using PD.Services.Projections.Rules.ContractSettlementComputations;
using PD.Models.AppViewModels;

namespace PD.Services.Projections
{
    public class FacultyProjectionService: ProjectionService
    {
        public FacultyProjectionService(ApplicationDbContext db, IPdDataProtector dataProtector)
           : base(db, dataProtector)
        {
        }

        /// <summary>
        /// Calculates the salary of the given position assignment for the specified target year.
        /// Note that this method relies on the necessary compensation components to be already loaded on to the given "pa"
        /// </summary>
        /// <param name="pa">The PositionAssignment to be updated.</param>
        /// <param name="targetDate">The target date.</param>
        /// <param name="projectionModeFlag">If set to true, then updates the projected salary. Otherwise, updates the confirmed salary </param>
        public override PositionAssignment CalculateSalary(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag)
        {
            //This service method is specifically designed only for faculty positions. Therefore, all
            //positions must be faculty positions.
            Faculty position = pa.Position as Faculty;
            if (position == null)
                throw new Exception("Error: a faculty position is expected.");

            //Target year's merit. Give priority for confirmed merits.
            Merit merit = pa.GetCompensation<Merit>(targetDate, PositionAssignment.eCompensationRetrievalPriority.ConfirmedFirst);

            if (position.Rank == Faculty.eRank.Professor1 || position.Rank == Faculty.eRank.Professor2 || position.Rank == Faculty.eRank.Professor3)
            {
                pa = CalculateSalaryFullProfessor(pa, targetDate, projectionModeFlag);
            }
            else
            {
                if (merit.IsPromoted)
                {
                    //TODO: handle prmotions
                }
                else
                {
                    Salary newSalary = ProjectRegularSalary(pa, targetDate);
                }
            }

            return pa;
        }

        public PositionAssignment CalculateSalaryFullProfessor(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag)
        {
            return pa;
        }

        protected List<AbstractProjectionRule> GetSalaryCalculationRules()
        {
            //Creating instances of salary-calculation rules in the correct order of applying them
            List<AbstractProjectionRule> rules = new List<AbstractProjectionRule>()
            {
                new ComputeContractSettlement(Db, _dataProtector),
                new ComputeMerit(Db, _dataProtector) /*,
                new ComputeFullProfessorMerit(Db)*/,
                new AggregateBaseSalaryComponents(Db, _dataProtector)/*,
                new HandleNonFullProfessorPromotions(Db),
                new HandleUpperSalaryLimits(Db)*/
            };

            return rules;
        }

        public void ComputeSalaries(
            int positionAssignmentId,
            DateTime from,
            DateTime to,
            int stepInMonths = 12,
            List<AbstractProjectionRule> computationRules = null,
            ComputationResult statusAggregator = null)
        {
            //Select all faculty position assignments which are active by the given target date
            PositionAssignment positionAssignment = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa => pa.Person)
                .Include(pa => pa.Compensations)
                .Where(pa => pa.Id == positionAssignmentId)
                .FirstOrDefault();

            if (positionAssignment == null)
                return;

            for(DateTime targetDate = from; targetDate <= to; targetDate = targetDate.AddMonths(stepInMonths))
                ComputeSalaries(positionAssignment, targetDate, computationRules, statusAggregator);
        }

        public void ComputeSalaries(
            PositionAssignment pa,
            DateTime targetDate,
            List<AbstractProjectionRule> computationRules = null,
            ComputationResult statusAggregator = null)
        {
            if (computationRules == null)
                computationRules = GetSalaryCalculationRules();

            foreach (AbstractProjectionRule rule in computationRules)
            {
                try
                {
                    rule.Execute(ref pa, targetDate);
                    if (statusAggregator != null)
                        ++statusAggregator.SuccessCount;
                }
                catch (Exception ex)
                {
                    if (statusAggregator != null)
                    {
                        ++statusAggregator.ErrorCount;
                        statusAggregator.Errors.Add(ex.Message);
                    }
                }
            }
            Db.SaveChanges();
        }

        /// <summary>
        /// Projects the faculty salaries for the period identified by the targetDate.
        /// The salary information for the previous salary period must have been calculated 
        /// for each employee.
        /// </summary>
        /// <param name="targetDate">The target date.
        /// </param>
        public ComputationResult ProjectFacultySalaries(DateTime targetDate, bool clearPastAuditLog)
        {
            //Creating instances of salary-calculation rules in the correct order of applying them
            List<AbstractProjectionRule> rules = GetSalaryCalculationRules();

            //Select all faculty position assignments which are active by the given target date
            IQueryable<PositionAssignment> query = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa => pa.Person)
                .Include(pa => pa.Compensations);

            if (clearPastAuditLog)
                query = query.Include(pa => pa.AuditTrail);

            query = query.Where(pa => pa.Position is Faculty
                            && pa.StartDate <= targetDate
                            && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate));

            List<PositionAssignment> facultyPositions = query.ToList();

            //Iterating through each position assignment
            ComputationResult result = new ComputationResult();
            foreach(PositionAssignment pa in facultyPositions)
            {
                if (clearPastAuditLog)
                    pa.AuditTrail.Clear();

                ComputeSalaries(pa, targetDate, rules, result);
            }

            result.Successes.Add(string.Format("{0} rule-executions completed successfully", result.SuccessCount));

            return result;
        }
    }
}
