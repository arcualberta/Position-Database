using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class PositionAccount
    {
        [Key]
        public int Id { get; set; }

        public int ChartStringId { get; set; }
        public ChartString ChartString { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public decimal Value { get; set; }
        public bool IsPercentage { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
