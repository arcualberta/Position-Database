using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PositionDatabase.Shared
{
    public class SalaryComponent
    {
        public Guid Id { get; set; }

        [Column(TypeName = "decimal(19,4)")]
        public decimal Value { get; set; }

        public double CurrentStep { get; set; }

        public Guid PositionId { get; set; }
        public Position Position { get; set; }
    }
}
