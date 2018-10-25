using PD.Models.Positions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class PositionAccount
    {
        [Key]
        public int Id { get; set; }

        public int ChartStringId { get; set; }
        public virtual ChartString ChartString { get; set; }

        public int PositionId { get; set; }
        public virtual Position Position { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100)]
        public decimal ValuePercentage { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
