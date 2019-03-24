using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Adjustment : Compensation
    {
        [Display(Name = "Is this a component of the base salary?")]
        public bool IsBaseSalaryComponent { get; set; }

        public Adjustment() 
            :base("Adjustment")
        {

        }
        public override Compensation Clone()
        {
            Adjustment child = Clone<Adjustment>();
            child.Name = Name;
            child.IsBaseSalaryComponent = IsBaseSalaryComponent;

            return child;
        }

    }
}
