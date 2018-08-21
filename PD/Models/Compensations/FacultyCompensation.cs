using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class FacultyCompensation : Compensation
    {
        public double MeritDecision { get; set; }
        public string MeritReason { get; set; }
        public decimal ContractSuppliment { get; set; }
        public decimal Merit { get; set; }
        public decimal SpecialAdjustment { get; set; }
        public decimal MarketSupplement { get; set; }
    }
}
