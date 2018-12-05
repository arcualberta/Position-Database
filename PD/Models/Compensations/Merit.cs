﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public class Merit : Compensation
    {
        [Display(Name = "Is Promoted?")]
        public bool IsPromoted { get; set; }

        public decimal Decision { get; set; }
    }
}