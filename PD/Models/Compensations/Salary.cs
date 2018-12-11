using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Salary : Compensation
    {
        [Display(Name = "Is Maxed")]
        public bool IsMaxed { get; set; }

        public override Compensation Clone()
        {
            Salary child = Clone<Salary>();
            child.IsMaxed = IsMaxed;

            return child;
        }
    }
}
