using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Positions
{
    public class Position
    {
        public enum eWorkload
        {
            [Display(Name = "Full Time")]
            F,

            [Display(Name = "Part Time")]
            P
        }

        public enum eContractType
        {
            R,
            S
        }


        /// <summary>
        /// Unique System ID.
        /// </summary>
        /// <value>
        /// </value>
        [Key]
        public int Id { get; set; }

        [Display(Name = "Position Number")]
        public string Number { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public eWorkload Workload { get; set; }

        public eContractType ContractType { get; set; }

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

        public int? PersonId { get; set; }
        public Person Person { get; set; }

        public virtual ICollection<PositionAccount> PositionAccounts { get; set; } = new List<PositionAccount>();

        public virtual ICollection<PositionAssignment> PositionAssignments { get; set; } = new List<PositionAssignment>();
    }
}
