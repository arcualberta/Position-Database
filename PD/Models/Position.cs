using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class Position
    {
        public enum ePositionType {
            [Display(Name ="Full Time Continuing")]
            FullTimeContinuing,
            [Display(Name = "Full Time Contract")]
            FullTimeContract,
            [Display(Name = "Part Time")]
            PartTime
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
        public ePositionType PositionType { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date of this position record. There can be multiple positions
        /// with the same position number but covering different time periods.
        /// </value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date of this position record. There can be multiple positions
        /// with the same position number but covering different time periods.
        /// </value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        /// <value>
        /// The effective date of this position record. The effective date can be earlier
        /// than the start date in situations where a change is made to the position but
        /// make this effective from the past (backdated)
        /// </value>
        public DateTime EffectiveDate { get; set; }


        /// <summary>
        /// Gets or sets the accounts.
        /// </summary>
        /// <value>
        /// The accounts that funds this position
        /// </value>
        public virtual ICollection<PositionAccount> Accounts { get; set; }

        public int CurrentPersonId { get; set; }

        /// <summary>
        /// Gets or sets the current person.
        /// </summary>
        /// <value>
        /// The current person who is associated with this position at the moment.
        /// </value>
        public PersonPosition CurrentPerson { get; set; }
    }
}
