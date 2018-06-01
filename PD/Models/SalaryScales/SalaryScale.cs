using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.SalaryScales
{
    public class SalaryScale
    {
        public int Id { get; set; }

        public string Guid { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ATB (Across the Board) proportion.
        /// </summary>
        /// <value>
        /// The ATB proportion; minimum 0 and maximum 100.
        /// </value>
        [Range(0.0, 100)]
        public decimal ATBPercentatge { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
