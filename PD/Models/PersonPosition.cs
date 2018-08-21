using PD.Models.Compensations;
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
    public class PersonPosition
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

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(0.0, 100)]
        [Display(Name ="Percentage (0 - 100)")]
        public double Percentage { get; set; }
        public eStatus Status { get; set; }

        public int? PersonId { get; set; }
        public virtual Person Person { get; set; }
        public int? PositionId { get; set; }
        public virtual Position Position {get;set;}

        public virtual ICollection<Compensation> Compensations { get; set; } = new List<Compensation>();
    }
}
