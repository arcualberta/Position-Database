using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    [Display(Name = "Contract Settlement")]
    public class ContractSettlement : Compensation
    {
        public decimal Percentage { get; set; }
    }
}
