using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class BudgetSummary
    {
        public DateTime Date { get; set; } 
        public decimal? Budget { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Merits { get; set; }
        public decimal? ContractSettlement { get; set; }
        public decimal? Adjustments { get; set; }
        public int? PositionCount { get; set; }

    }
}
