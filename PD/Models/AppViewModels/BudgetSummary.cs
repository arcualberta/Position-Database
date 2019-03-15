using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class BudgetSummary
    {
        public DateTime Date { get; set; } 
        public decimal Budget { get; set; }
        public decimal PositionCount { get; set; }

    }
}
