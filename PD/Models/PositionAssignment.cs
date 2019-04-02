using PD.Models.Compensations;
using PD.Models.Positions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    /// <summary>
    /// PersonPosition
    /// </summary>
    public class PositionAssignment
    {
        public enum eStatus
        {
            Active = 1,
            [Display(Name ="Pre-retirement")]
            PreRetirement,
            [Display(Name ="Post-retirement")]
            PostRetirement,
            Resignation,
            Termination,
            [Display(Name ="Leave without pay")]
            LeaveWithoutPay
        }

        ////////////////public enum eCompensationRetrievalPriority { ProjectionFirst, ConfirmedFirst }

        [Key]
        public int Id { get; set; }

        public eStatus Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Display(Name = "Month in which a new salary year begins")]
        [Range(1, 12)]
        public int SalaryCycleStartMonth { get; set; }
        [Display(Name = "Date in which a new salary year begins")]
        [Range(1, 31)]
        public int SalaryCycleStartDay { get; set; }

        public int? PositionId { get; set; }
        public virtual Position Position {get;set;}

        public int? PersonId { get; set; }
        public Person Person { get; set; }

        public virtual ICollection<Compensation> Compensations { get; set; } = new List<Compensation>();

        public virtual ICollection<AuditRecord> AuditTrail { get; set; } = new LinkedList<AuditRecord>();

        public int? PredecessorId { get; set; }
        /// <summary>
        /// Predecessor to this position assignment. 
        /// When the Status of a PositionAssignment is changed (e.g. a person goes from active to pre-retirement
        /// then instead of changing the Status of the original assignment, an end-date must be put on the original
        /// PositionAssignment and a new PositionAssignment must be created with the new Status. In this case the
        /// Predecessor of the new PositionAssignment must refer to the original PositionAssignment.
        /// </summary>
        /// <value>
        /// </value>
        public PositionAssignment Predecessor { get; set; }

        public int? SuccessorId { get; set; }
        /// <summary>
        /// Succcessor of this position assignment. 
        /// When the Status of a PositionAssignment is changed (e.g. a person goes from active to pre-retirement
        /// then instead of changing the Status of the original assignment, an end-date must be put on the original
        /// PositionAssignment and a new PositionAssignment must be created with the new Status. In this case the
        /// Succcessor of the original PositionAssignment must refer to the new PositionAssignment.
        /// </summary>
        /// <value>
        /// </value>
        public PositionAssignment Successor { get; set; }


        public bool IsPaidOn(DateTime targetDate)
        {
            //Position assignments in following status values get paid
            //if the given targetDate is within their start and end dates
            bool pay = (Status == eStatus.Active
                       || Status == eStatus.PostRetirement
                       || Status == eStatus.PreRetirement
                       )
                       && StartDate <= targetDate
                       && (EndDate.HasValue == false || EndDate >= targetDate);

            return pay;
        }

        /////////////////// <summary>
        /////////////////// Returns the projected or confirmed compensation of the give type T for the target date
        /////////////////// from the compensations already loaded into memory.        
        /////////////////// </summary>
        /////////////////// <typeparam name="T"></typeparam>
        /////////////////// <param name="targetDate">The target date.</param>
        /////////////////// <param name="isProjection">if set to <c>true</c> then returns the projected compensation. Otherwise, retutns the confirmed compensation.</param>
        /////////////////// <returns></returns>
        ////////////////[Obsolete("This method is obsolete. Call GetCompensations<T>(DateTime targetDate) instead.", false)]
        ////////////////public T GetCompensation<T>(DateTime targetDate, bool isProjection) where T : Compensation
        ////////////////{
        ////////////////    return Compensations
        ////////////////        .Where(c => c is T && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == isProjection)
        ////////////////        .FirstOrDefault() as T;
        ////////////////}

        /// <summary>
        /// Returns all compensations of given type which are active at the given target date.
        /// </summary>
        /// <typeparam name="T">Type of compensations requested.</typeparam>
        /// <param name="targetDate">The target date.</param>
        /// <returns></returns>
        public IEnumerable<T> GetCompensations<T>(DateTime targetDate) where T : Compensation
        {
            return Compensations
                .Where(c => c is T
                    && c.StartDate <= targetDate
                    && (c.EndDate.HasValue == false || c.EndDate > targetDate))
                .Select(c => c as T);
        }

        public IEnumerable<Compensation> GetCompensations(DateTime targetDate)
        {
            return Compensations
                .Where(c => 
                    c.StartDate <= targetDate
                    && (c.EndDate.HasValue == false || c.EndDate > targetDate));
        }

        /////////////////// <summary>
        /////////////////// Returns the confirmed compensation of the give type T for the target date
        /////////////////// from the compensations already loaded into memory. If it's not available but a projected one available that matches the criteria, then 
        /////////////////// returns that projected one.
        /////////////////// </summary>
        /////////////////// <typeparam name="T"></typeparam>
        /////////////////// <param name="targetDate">The target date.</param>
        /////////////////// <returns></returns>
        ////////////////public T GetCompensation<T>(DateTime targetDate, eCompensationRetrievalPriority priority) where T : Compensation
        ////////////////{
        ////////////////    //Set the flag to true if we should give a pririty for retrieving projections, and set false otherwise.
        ////////////////    bool flag = priority == eCompensationRetrievalPriority.ProjectionFirst;

        ////////////////    //Get the projected compensation
        ////////////////    T compensation = GetCompensation<T>(targetDate, flag); 

        ////////////////    //If the projected compensation does not exist then get the actual/confirmed compensation
        ////////////////    if(compensation == null)
        ////////////////        compensation = GetCompensation<T>(targetDate, !flag);

        ////////////////    //If the requested compensation is not found in the current position assignment BUT if a 
        ////////////////    //pedecessor position assignment exists, then obtain the requested compensation from that 
        ////////////////    //predecessor.
        ////////////////    if (compensation == null && Predecessor != null)
        ////////////////        compensation = Predecessor.GetCompensation<T>(targetDate, priority);

        ////////////////    return compensation;
        ////////////////}


        /// <summary>
        /// Returns all adjustment instances which are part of the base salary for the period covering the target date.
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <param name="isBaseSalaryComponents">if set to <c>true</c> then returns adjustments that are part of the base salary. 
        /// Otherwise, returns adjustments that are NOT part of the base salary.
        /// </param>
        /// <returns></returns>
        public IEnumerable<Adjustment> GetAdjustments(DateTime targetDate, bool isBaseSalaryComponents)
        {
            IEnumerable<Adjustment> adjustments = Compensations
                .Where(c => c is Adjustment && c.StartDate <= targetDate && c.EndDate >= targetDate && (c as Adjustment).IsBaseSalaryComponent == isBaseSalaryComponents)
                .ToList()
                .Select(c => c as Adjustment)
                .ToList();

            return adjustments;
        }
        /// <summary>
        /// Gets the salary-cycle start date of the period which includes the given target date.
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <returns></returns>
        public DateTime GetSalaryCycleStartDate(DateTime targetDate)
        {
            DateTime cycleStartDate = new DateTime(targetDate.Year, SalaryCycleStartMonth, SalaryCycleStartDay);

            if (cycleStartDate > targetDate)
                cycleStartDate = cycleStartDate.AddYears(-1);

            return cycleStartDate;
        }

        /// <summary>
        /// Gets the salary-cycle end date of the period which includes the given target date.
        /// </summary>
        /// <param name="targetDate">The target date.</param>
        /// <returns></returns>
        public DateTime GetSalaryCycleEndDate(DateTime targetDate)
        {
            return GetSalaryCycleStartDate(targetDate).AddYears(1).AddDays(-1);
        }

        public void LogError(string message, DateTime targetDate)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Error)
            {
                Message = message,
                SalaryCycleStartDate = GetSalaryCycleStartDate(targetDate),
                SalaryCycleEndDate = GetSalaryCycleEndDate(targetDate)
            };
            AuditTrail.Add(record);
        }

        public void LogWarning(string message, DateTime targetDate)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Warning)
            {
                Message = message,
                SalaryCycleStartDate = GetSalaryCycleStartDate(targetDate),
                SalaryCycleEndDate = GetSalaryCycleEndDate(targetDate)
            };
            AuditTrail.Add(record);
        }

        public void LogInfo(string message, DateTime targetDate)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Info)
            {
                Message = message,
                SalaryCycleStartDate = GetSalaryCycleStartDate(targetDate),
                SalaryCycleEndDate = GetSalaryCycleEndDate(targetDate)
            };
            AuditTrail.Add(record);
        }
    }
}
