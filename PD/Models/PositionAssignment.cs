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

        [Key]
        public int Id { get; set; }

        public eStatus Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? PositionId { get; set; }
        public virtual Position Position {get;set;}

        public int? PersonId { get; set; }
        public Person Person { get; set; }

        public virtual ICollection<Compensation> Compensations { get; set; } = new List<Compensation>();

        public virtual ICollection<ChangeLog> ChangeLog { get; set; } = new LinkedList<ChangeLog>();


        /// <summary>
        /// Returns the projected or confirmed compensation of the give type T for the target date
        /// from the compensations already loaded into memory.        
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targetDate">The target date.</param>
        /// <param name="isProjection">if set to <c>true</c> [is projection].</param>
        /// <returns></returns>
        public T GetCompensation<T>(DateTime targetDate, bool isProjection) where T : Compensation
        {
            return Compensations
                .Where(c => c is T && c.StartDate <= targetDate && c.EndDate >= targetDate && c.IsProjection == isProjection)
                .FirstOrDefault() as T;
        }
    }
}
