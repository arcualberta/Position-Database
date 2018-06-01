using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class PositionAccount : ChartString
    {
        public decimal Value { get; set; }
        public bool IsPercentage { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

    }
}
