using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class FacultyCompensation : Compensation
    {
        public decimal SpecialAdjustment { get; set; }
        public decimal ContractSettlement { get; set; }

    }
}
