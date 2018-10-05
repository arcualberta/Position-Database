using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Adjustment
    {
        [Key]
        public int Id { get; set; }

        public int PersonPositionId { get; set; }
        public PersonPosition PersonPosition { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
