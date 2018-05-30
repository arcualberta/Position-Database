using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PAD.Models
{
    public class PositionAccount
    {
        [Key]
        public int Id { get; set; }

        [Range(0.0, 1.0)]
        public decimal Proportion { get; set; }

        public int ChartStringId { get; set; }
        public ChartString ChartString { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

    }
}
