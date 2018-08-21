using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class Position
    {
        public enum ePositionWorkload {
            [Display(Name ="Full Time")]
            F = 1,
            [Display(Name = "Part Time")]
            P
        }

        public enum ePositionContract
        {
            [Display(Name = "Regular")]
            R = 1,
            [Display(Name = "Sessional")]
            S
        }

        /// <summary>
        /// Unique System ID.
        /// </summary>
        /// <value>
        /// </value>
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public ePositionWorkload PositionWorkload { get; set; }
        public ePositionContract PositionContract { get; set; }
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date of this position record. There can be multiple positions
        /// with the same position number but covering different time periods.
        /// </value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date of this position record. There can be multiple positions
        /// with the same position number but covering different time periods.
        /// </value>
        public DateTime? EndDate { get; set; }

        /////// <summary>
        /////// Gets or sets the effective date.
        /////// </summary>
        /////// <value>
        /////// The effective date of this position record. The effective date can be earlier
        /////// than the start date in situations where a change is made to the position but
        /////// make this effective retrospectively. 
        /////// </value>
        ////public DateTime? EffectiveDate { get; set; }


        public virtual ICollection<PositionAccount> PositionAccounts { get; set; } = new List<PositionAccount>();

        public virtual ICollection<PersonPosition> PersonPositions { get; set; } = new List<PersonPosition>();
    }
}
