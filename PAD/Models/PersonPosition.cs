using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PAD.Models
{
    /// <summary>
    /// PersonPosition
    /// </summary>
    public class PersonPosition
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// List of position records that are attached to the current person position.
        /// </summary>
        public virtual ICollection<Position> Positions {get;set;}

    }
}
