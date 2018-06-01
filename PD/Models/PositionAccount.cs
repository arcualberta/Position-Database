using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class PositionAccount : ChartString
    {
        [Range(0.0, 1.0)]
        public decimal Proportion { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

    }
}
