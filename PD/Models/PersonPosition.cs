﻿using System;
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
        [Key]
        public int Id { get; set; }

        public int? PersonId { get; set; }
        public Person Person { get; set; }

        /// <summary>
        /// List of position records that are attached to the current person position.
        /// </summary>
        public virtual ICollection<Position> Positions {get;set;}

    }
}
