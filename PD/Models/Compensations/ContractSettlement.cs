using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class ContractSettlement : Compensation
    {
        public override Compensation Clone()
        {
            return Clone<ContractSettlement>();
        }
    }
}
