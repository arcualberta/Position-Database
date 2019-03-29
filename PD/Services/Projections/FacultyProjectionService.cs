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
                new ComputeMerit(Db, _dataProtector) ,
                new ComputeFullProfessorMerit(Db, _dataProtector),
                new AggregateBaseSalaryComponents(Db, _dataProtector),
                new HandleNonFullProfessorPromotions(Db, _dataProtector),
                new HandleUpperSalaryLimits(Db, _dataProtector)
            };

            return rules;
        }

        public void ComputeSalaries(
            int positionAssignmentId,
            DateTime from,
            DateTime to,
            int stepInMonths = 12,
            ComputationResult statusAggregator = null,
            List<AbstractProjectionRule> computationRules = null)
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
                ComputeCompensation(positionAssignment, targetDate, statusAggregator, computationRules);
        }

        public bool ComputeCompensation(
            int positionAssignmentId,
            DateTime targetDate,
            ComputationResult statusAggregator = null,
            List<AbstractProjectionRule> computationRules = null)
        {
            PositionAssignment positionAssignment = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa => pa.Person)
                .Include(pa => pa.Compensations)
                .Where(pa => pa.Id == positionAssignmentId)
                .FirstOrDefault();

            if(positionAssignment == null)
            {
                if (statusAggregator != null)
                    statusAggregator.Errors.Add(string.Format("No position assignment with id {0} found.", positionAssignmentId));
                return false;
            }

            return ComputeCompensation(positionAssignment, targetDate, statusAggregator, computationRules);
        }

        public bool ComputeCompensation(
            PositionAssignment pa,
            DateTime targetDate,
            ComputationResult statusAggregator = null,
            List<AbstractProjectionRule> computationRules = null)
        {
            if (computationRules == null)
                computationRules = GetSalaryCalculationRules();

            bool updated = false;
            foreach (AbstractProjectionRule rule in computationRules)
            {
                try
                {
                    rule.Execute(ref pa, targetDate);
                    if (statusAggregator != null)
                        ++statusAggregator.SuccessCount;
                    updated = true;
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

            return updated;
        }

        /// <summary>
        /// Projects the faculty salaries for the period identified by the targetDate.
        /// The salary information for the previous salary period must have been calculated 
        /// for each employee.
        /// </summary>
        /// <param name="targetDate">The target date.
        /// </param>
        public ComputationResult ProjectFacultySalaries(DateTime targetDate)
        {
            //Creating instances of salary-calculation rules in the correct order of applying them
            List<AbstractProjectionRule> rules = GetSalaryCalculationRules();

            //Select all faculty position assignments which are active by the given target date
            IQueryable<PositionAssignment> query = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa => pa.Person)
                .Include(pa => pa.Compensations)
                .Include(pa => pa.AuditTrail);

            query = query.Where(pa => pa.Position is Faculty
                            && pa.StartDate <= targetDate
                            && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate));

            List<PositionAssignment> facultyPositions = query.ToList();

            //Iterating through each position assignment
            ComputationResult result = new ComputationResult();
            foreach(PositionAssignment pa in facultyPositions)
            {
                DateTime salaryCycleStart = pa.GetCycleStartDate(targetDate);
                List<AuditRecord> pastMatchingRecords = pa.AuditTrail
                    .Where(r => r.SalaryCycleStartDate == salaryCycleStart && r.SalaryCycleEndDate == salaryCycleStart.AddYears(1).AddDays(-1))
                    .ToList();

                foreach (var au in pastMatchingRecords)
                    au.IsHistoric = true;

                ComputeCompensation(pa, targetDate, result, rules);
            }

            result.Successes.Add(string.Format("{0} rule-executions completed successfully", result.SuccessCount));

            return result;
        }
    }
}
