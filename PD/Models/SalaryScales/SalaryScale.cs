using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.SalaryScales
{
    public class SalaryScale
    {
        [Key]
        public int Id { get; set; }

        public string Category { get; set; }

        [Display(Name = "Minimum Salary")]
        public decimal Minimum { get; set; }

        [Display(Name = "Maximum Salary")]
        public decimal Maximum { get; set; }

        [Display(Name = "Salary Step Dollar Value")]
        public decimal StepValue { get; set; }

        [Display(Name = "Contract Settlement")]
        [Range(0.0, 100)]
        public decimal ContractSettlement { get; set; }

        [Display(Name = "Default Merit Decision")]
        public decimal DefaultMeritDecision { get; set; }

        [Display(Name = "Is Projection?")]
        public bool IsProjection { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
