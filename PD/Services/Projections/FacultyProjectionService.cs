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


        //////public void ProjectSalaries(DateTime targetDate)
        //////{
        //////    decimal defaultMeritDecision = 1;
        //////    bool updateDatabase = true;

        //////    List<PositionAssignment> facultyPositions = Db.PositionAssignments
        //////        .Include(pa => pa.Position)
        //////        .Include(pa => pa.Person)
        //////        .Include(pa => pa.Compensations)
        //////        .Where(pa => pa.Position is Faculty && pa.StartDate <= targetDate && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate))
        //////        .ToList();

        //////    foreach (PositionAssignment pa in facultyPositions)
        //////    {
        //////        try
        //////        {
        //////            PositionAssignment updated = ProjectSalary(pa, targetDate, defaultMeritDecision, updateDatabase);
        //////        }
        //////        catch (Exception ex)
        //////        {

        //////        }
        //////    }

        //////}


        public void ProjectSalaries(DateTime targetDate)
        {
            List<PositionAssignment> facultyPositions = Db.PositionAssignments
                .Include(pa => pa.Position)
                .Include(pa=>pa.Person)
                .Include(pa => pa.Compensations)
                .Include(pa => pa.AuditTrail)
                .Where(pa => pa.Position is Faculty && pa.StartDate <= targetDate && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate))
                .ToList();

            List<AbstractProjectionRule> rules = new List<AbstractProjectionRule>()
            {
                new ComputeContractSettlement(Db),
                new ComputeMerit(Db),
                new ComputeFullProfessorMerit(Db),
                new AggregateBaseSalaryComponents(Db),
                new HandleNonFullProfessorPromotions(Db),
                new HandleUpperSalaryLimits(Db)
            };


            foreach(PositionAssignment posAssignment in facultyPositions)
            {
                PositionAssignment pa = posAssignment;

                //Removing non-log type messges from the audit trail
                var oldMessages = pa.AuditTrail.Where(au =>
                    au.AuditType == AuditRecord.eAuditRecordType.Info ||
                    au.AuditType == AuditRecord.eAuditRecordType.Warning ||
                    au.AuditType == AuditRecord.eAuditRecordType.Error)
                .ToList();
                foreach (var message in oldMessages)
                    pa.AuditTrail.Remove(message);

                foreach (AbstractProjectionRule rule in rules)
                {
                    try
                    {
                        rule.Execute(ref pa, targetDate);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                Db.SaveChanges();

            }

        }


    }
}
