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
    public abstract class AbstractProjectionRule
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ApplicationDbContext Db { get; set; }

        public IPdDataProtector Dp { get; set; }


        public abstract bool Execute(ref PositionAssignment pa, DateTime targetDate);

        public AbstractProjectionRule(ApplicationDbContext db, IPdDataProtector dp, string name, string description)
        {
            Name = name;
            Description = description;
            Db = db;
            Dp = dp;
        }

        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            SalaryScale scale = Db.SalaryScales
                .Where(sc => sc.StartDate <= targetDate && sc.EndDate >= targetDate && sc.Name == positionTitle)
                .FirstOrDefault();

            if (scale == null)
                throw new Exception(string.Format("Salary scale not found for the year of {0} for the position of {1}",
                    targetDate, positionTitle));

            return scale;
        }

        public Salary GetPastSalary(PositionAssignment pa, DateTime targetDate)
        {
            DateTime lastDayOfPastSalaryCycle = pa.GetCycleStartDate(targetDate).AddDays(-1);

            Salary pastSalary = pa.Compensations
                .Where(c => c is Salary
                    && c.StartDate <= lastDayOfPastSalaryCycle
                    && (c.EndDate.HasValue == false || c.EndDate >= lastDayOfPastSalaryCycle))
                .FirstOrDefault() as Salary;

            if (pastSalary == null)
                throw new Exception(string.Format("Past year's salary not found for the target date of {0} for the position of {1} of {2}",
                    targetDate, pa.Position.Title, Dp.Decrypt(pa.Person.Name)));

            return pastSalary;
        }

        public bool PromoteToFacultyPosition(ref PositionAssignment pa, string newPositionTitle, DateTime promotionStartDate)
        {
            try
            {
                //Putting an end date for the current position assignment and it's associated position
                PositionAssignment oldPositionAssignment = pa;
                oldPositionAssignment.EndDate = promotionStartDate.AddDays(-1);
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
                Db.PositionAssignments.Add(pa);

                //Linking the previous position assignment as the predecessor for this position assignment
                pa.Predecessor = oldPositionAssignment;

                Position position = new Faculty()
                {
                    ContractType = oldPositionAssignment.Position.ContractType,
                    Number = oldPositionAssignment.Position.Number,
                    Rank = Enum.Parse<Faculty.eRank>(newPositionTitle),
                    StartDate = pa.StartDate,
                    Title = newPositionTitle,
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
                    .Where(c => c.StartDate <= promotionStartDate && c.EndDate >= promotionStartDate)
                    .ToList();
                foreach (Compensation c in compensations)
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

                    //If this compensation is a merit, then we should set it's IsPromoted flag to false
                    //because we already created the promoted position here.
                    if (c is Merit)
                        (c as Merit).IsPromoted = false;
                }

                pa.LogInfo("New position created", pa.GetCycleYearRange(promotionStartDate));

                return true;
            }
            catch (Exception ex)
            {
                pa.LogError(ex.Message, pa.GetCycleYearRange(promotionStartDate));
                return false;
            }
        }

    }
}
