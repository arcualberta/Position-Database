using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Salary : Compensation
    {
        [Display(Name = "Current Salary Step"]
        public decimal CurrentSalaryStep { get; set; }

        [Display(Name = "Year-end Merit Decision"]
        public decimal MeritDecision { get; set; }

        [Display(Name = "Year-end Merit Reason"]
        public string MeritReason { get; set; }

        [Display(Name = "Year-end Merit"]
        public decimal Merit { get; set; }

        public Salary()
        {
            Type = "Salary";
        }
    }
}
