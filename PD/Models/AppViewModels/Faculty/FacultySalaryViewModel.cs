using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels.Faculty
{
    public class FacultySalaryViewModel : SalaryViewModel
    {
        public decimal MeritDecision { get; set; }
        public string MeritReason { get; set; }
        public decimal ContractSettlement { get; set; }
        public decimal Merit { get; set; }
        public decimal SpecialAdjustment { get; set; }
        public decimal MarketSupplement { get; set; }
        public bool IsPromoted { get; set; }
        public string Notes { get; set; }
    }
}
