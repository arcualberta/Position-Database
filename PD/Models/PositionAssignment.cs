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

        public enum eCompensationRetrievalPriority { ProjectionFirst, ConfirmedFirst }

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
        public PositionAssignment Predecessor { get; set; }

        /// <summary>
        /// Returns the projected or confirmed compensation of the give type T for the target date
        /// from the compensations already loaded into memory.        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetDate">The target date.</param>
        /// <param name="isProjection">if set to <c>true</c> then returns the projected compensation. Otherwise, retutns the confirmed compensation.</param>
        /// <returns></returns>
        public T GetCompensation<T>(DateTime targetDate, bool isProjection) where T : Compensation
        {
            return Compensations
                .Where(c => c is T && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == isProjection)
                .FirstOrDefault() as T;
        }

        /// <summary>
        /// Returns the confirmed compensation of the give type T for the target date
        /// from the compensations already loaded into memory. If it's not available but a projected one available that matches the criteria, then 
        /// returns that projected one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetDate">The target date.</param>
        /// <returns></returns>
        public T GetCompensation<T>(DateTime targetDate, eCompensationRetrievalPriority priority) where T : Compensation
        {
            //Set the flag to true if we should give a pririty for retrieving projections, and set false otherwise.
            bool flag = priority == eCompensationRetrievalPriority.ProjectionFirst;

            //Get the projected compensation
            T compensation = GetCompensation<T>(targetDate, flag); 

            //If the projected compensation does not exist then get the actual/confirmed compensation
            if(compensation == null)
                compensation = GetCompensation<T>(targetDate, !flag);

            //If the requested compensation is not found in the current position assignment BUT if a 
            //pedecessor position assignment exists, then obtain the requested compensation from that 
            //predecessor.
            if (compensation == null && Predecessor != null)
                compensation = Predecessor.GetCompensation<T>(targetDate, priority);

            return compensation;
        }


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

        public DateTime GetCycleStartDate(DateTime targetDate)
        {
            DateTime cycleStartDate = new DateTime(targetDate.Year, SalaryCycleStartMonth, SalaryCycleStartDay);

            if (cycleStartDate > targetDate)
                cycleStartDate = cycleStartDate.AddYears(-1);

            return cycleStartDate;

        }

        public void LogError(string message)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Error)
            {
                Message = message             
            };
            AuditTrail.Add(record);
        }

        public void LogWarning(string message)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Warning)
            {
                Message = message
            };
            AuditTrail.Add(record);
        }

        public void LogInfo(string message)
        {
            AuditRecord record = new AuditRecord(AuditRecord.eAuditRecordType.Info)
            {
                Message = message
            };
            AuditTrail.Add(record);
        }


    }
}
