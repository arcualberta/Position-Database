using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.Positions;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public abstract class AbstractProjectionRule
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ApplicationDbContext Db { get; set; }

        public IPdDataProtector Dp { get; set; }

        public readonly SalaryScaleService _salaryScaleService;

        public abstract bool Execute(ref PositionAssignment pa, DateTime targetDate);

        public AbstractProjectionRule(ApplicationDbContext db, IPdDataProtector dp, SalaryScaleService salaryScaleService, string name, string description)
        {
            Name = name;
            Description = description;
            Db = db;
            Dp = dp;
            _salaryScaleService = salaryScaleService;
        }

        protected Person Person;
        public void Execute(Person person, DateTime targetDate)
        {
            Person = person;

            List<PositionAssignment> activePositionAssignments = person.PositionAssignments
                .Where(pa => pa.StartDate <= targetDate
                            && (pa.EndDate.HasValue == false || pa.EndDate >= targetDate)
                      )
                .ToList();

            foreach(PositionAssignment pa in activePositionAssignments)
            {
                PositionAssignment p = pa;
                Execute(ref p, targetDate);
            }
        }




        [Obsolete("AbstractProcessingRule.GetPastSalary deprecated. Use PositionAssignment.GetPastSalary method instead.")]
        public Salary GetPastSalary(PositionAssignment pa, DateTime targetDate)
        {
            return pa.GetPastSalary(targetDate);
        }


        [Obsolete("AbstractProcessingRule.GetSalary deprecated. Use PositionAssignment.GetSalary method instead.")]
        public Salary GetSalary(PositionAssignment pa, DateTime targetDate)
        {
            return pa.Compensations
                .Where(c => c is Salary
                    && c.StartDate <= targetDate
                    && (c.EndDate.HasValue == false || c.EndDate >= targetDate))
                .FirstOrDefault() as Salary;
        }

        /////// <summary>
        /////// Gets the start and end date for the contract settlement for the
        /////// period which include the given target date for a given position assignment.
        /////// </summary>
        /////// <param name="pa">The position assignment.</param>
        /////// <param name="targetDate">The target date.</param>
        /////// <returns>The start and end date for the contract settlement</returns>
        ////public DateTime[] GetContractSettlementPeriod(PositionAssignment pa, DateTime targetDate)
        ////{
        ////    //For faculty positions, the contract settlement period is as same as their salary cycle.
        ////    if(pa.Position is Faculty)
        ////    {
        ////        return new DateTime[] { pa.GetSalaryCycleStartDate(targetDate), pa.GetSalaryCycleEndDate(targetDate) };
        ////    }

        ////    throw new NotImplementedException("GetContractSettlementPeriod method is not implemented for non-faculty positions yet.");
        ////}

        public void PromoteToFacultyPosition(Person person, PositionAssignment pa, string newPositionTitle, DateTime promotionStartDate)
        {
            //Putting an end date for the current position assignment
            PositionAssignment oldPositionAssignment = pa;
            oldPositionAssignment.EndDate = promotionStartDate.AddDays(-1);

            //Creating a new position assignment
            pa = new PositionAssignment()
            {
                StartDate = oldPositionAssignment.EndDate.Value.AddDays(1),
                PersonId = oldPositionAssignment.PersonId,
                SalaryCycleStartDay = oldPositionAssignment.SalaryCycleStartDay,
                SalaryCycleStartMonth = oldPositionAssignment.SalaryCycleStartMonth,
                PositionId = oldPositionAssignment.PositionId,
                Status = oldPositionAssignment.Status
            };
            person.PositionAssignments.Add(pa);


            //Setting the new position as the successor in the old position assignment
            //and the old position as the predecessor in the new position assignment
            oldPositionAssignment.Successor = pa;
            pa.Predecessor = oldPositionAssignment;


            //Since we closed the old position assignment and created a new one, we
            //carry over all compensations recorded for the target year in the old one
            //into the new one
            List<Compensation> compensationsToBeCarriedOver = oldPositionAssignment
                .GetCompensations(promotionStartDate)
                .ToList();

            foreach (Compensation c in compensationsToBeCarriedOver)
            {
                //If the compensation start date is earlier than the start date of the new position assignment
                //then we split the compensation into two and set the end date of the first half to be a day prior
                //to the start date of the new position assignment and then we leave this first half with the 
                //old position assigment, and then we carry over the second half to the new position assignment.
                //If the start date of this compensation is as same as the start date of the new position assignment,
                //then we simply carry it over to the new position assignment (and remove it from the old one)

                Compensation comp;
                if (c.StartDate < pa.StartDate)
                {
                    //Clone the compensation and set the new start date
                    Compensation clone = c.Clone();
                    clone.StartDate = pa.StartDate.Value;
                    comp = clone;

                    //Set the end date to the original compensation
                    c.EndDate = pa.StartDate.Value.AddDays(-1);
                }
                else
                {
                    //We remove the compensation from the old position assignment
                    pa.Compensations.Remove(c);
                    comp = c;
                }

                //If this compensation is a merit, then we should set it's IsPromoted flag to false
                //because we already created the promoted position here.
                if (comp is Merit)
                    (comp as Merit).IsPromoted = false;

                //Now we add the compensation to the new position
                pa.Compensations.Add(comp);

            }//END: foreach (Compensation c in compensationsToBeCarriedOver)

            pa.LogInfo("New position created", promotionStartDate);
        }

        public bool PromoteToFacultyPosition(ref PositionAssignment pa, string newPositionTitle, DateTime promotionStartDate)
        {
            try
            {
                //Putting an end date for the current position assignment and it's associated position
                PositionAssignment oldPositionAssignment = pa;
                oldPositionAssignment.EndDate = promotionStartDate.AddDays(-1);

                //If the old position assignment has a successor, then use it
                //as the new assignment
                if (oldPositionAssignment.SuccessorId.HasValue)
                {
                    pa = Db.PositionAssignments
                        .Include(p => p.Position)
                        .Include(p => p.Compensations)
                        .Where(p => p.Id == oldPositionAssignment.SuccessorId)
                        .FirstOrDefault();
                }
                else
                {
                    //Creating a new position assignment
                    pa = new PositionAssignment()
                    {
                        StartDate = oldPositionAssignment.EndDate.Value.AddDays(1),
                        PersonId = oldPositionAssignment.PersonId,
                        SalaryCycleStartDay = oldPositionAssignment.SalaryCycleStartDay,
                        SalaryCycleStartMonth = oldPositionAssignment.SalaryCycleStartMonth,
                        PositionId = oldPositionAssignment.PositionId,
                        Status = oldPositionAssignment.Status
                    };
                    Db.PositionAssignments.Add(pa);

                    //Linking the previous position assignment as the predecessor for this position assignment
                    //////pa.Predecessor = oldPositionAssignment;
                    pa.PredecessorId = oldPositionAssignment.Id;

                    Position position = new Faculty()
                    {
                        ContractType = oldPositionAssignment.Position.ContractType,
                        Number = oldPositionAssignment.Position.Number,
                        Rank = Enum.Parse<Faculty.eRank>(newPositionTitle),
                        Title = newPositionTitle,
                        Workload = oldPositionAssignment.Position.Workload,
                        PrimaryDepartmentId = oldPositionAssignment.Position.PrimaryDepartmentId
                    };
                    position.PositionAssignments.Add(pa);
                    Db.Positions.Add(position);
                    //////pa.Position = position;

                    //Carrying over the same position accounts
                    List<PositionAccount> accounts = Db.PositionAccounts.Where(pacc => pacc.PositionId == oldPositionAssignment.PositionId).ToList();
                    foreach (var acc in accounts)
                        position.PositionAccounts.Add(acc);

                    Db.SaveChanges();
                    oldPositionAssignment.SuccessorId = pa.Id;
                }

                //Compensations to be carried over, i.e. all compensations recorded for the target year
                //in the old position account 
                List<Compensation> sourceCompensations = oldPositionAssignment.Compensations
                    .Where(c => c.StartDate <= promotionStartDate
                        && (c.EndDate.HasValue == false || c.EndDate >= promotionStartDate))
                    .ToList();
                foreach (Compensation c in sourceCompensations)
                {
                    //If the compensation start date is earlier than the start date of the new position assignment
                    //then we split the compensation into two and set the end date of the first half to be a day prior
                    //to the start date of the new position assignment and then we leave this first half with the 
                    //old position assigment, and then we carry over the second half to the new position assignment.
                    //If the start date of this compensation is as same as the start date of the new position assignment,
                    //then we simply carry it over to the new position assignment as a whole

                    Compensation comp;
                    if (c.StartDate < pa.StartDate)
                    {
                        c.EndDate = pa.StartDate.Value.AddDays(-1);
                        Compensation clone = c.Clone();
                        clone.StartDate = pa.StartDate.Value;
                        comp = clone;
                    }
                    else
                        comp = c;

                    //If this compensation is a merit, then we should set it's IsPromoted flag to false
                    //because we already created the promoted position here.
                    if (comp is Merit)
                        (comp as Merit).IsPromoted = false;

                    var match = pa.Compensations.Where(cc =>
                       cc.StartDate == comp.StartDate
                       && cc.EndDate == comp.EndDate
                       && cc.Name == comp.Name)
                        .FirstOrDefault();

                    if (match != null)
                        match.Value = comp.Value;
                    else
                        pa.Compensations.Add(comp);
                }

                ////foreach (Compensation comp in compensationsToBeCarriedOver)
                ////{
                ////    var match = pa.Compensations.Where(c =>
                ////       c.StartDate == comp.StartDate
                ////       && c.EndDate == comp.EndDate
                ////       && c.GetType() == comp.GetType())
                ////        .FirstOrDefault();

                ////    if (match != null)
                ////        match.Value = comp.Value;
                ////    else
                ////        pa.Compensations.Add(match);
                ////}

                pa.LogInfo("New position created", promotionStartDate);

                return true;
            }
            catch (Exception ex)
            {
                pa.LogError(ex.Message, promotionStartDate);
                return false;
            }
        }

    }
}
