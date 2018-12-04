using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Compensation
    {
        [Key]
        public int Id { get; set; }
        public string Year { get; set; }
        public decimal Value { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        public int PositionAssignmentId { get; set; }
        public virtual PositionAssignment PositionAssignment { get; set; }
     }
}
