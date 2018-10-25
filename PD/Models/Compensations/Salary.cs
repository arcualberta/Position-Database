using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Salary : Compensation
    {
        [Display(Name = "Year-end Merit Decision")]
        public decimal YearEndMeritDecision { get; set; }

        [Display(Name = "Year-end Merit Reason")]
        public string YearEndMeritReason { get; set; }

        [Display(Name = "Year-end Merit")]
        public decimal YearEndMerit { get; set; }

        [Display(Name = "Current Salary Step")]
        public decimal CurrentSalaryStep { get; set; }

        public Salary()
        {
        }
    }
}
