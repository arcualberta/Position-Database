﻿using PAD.Models.ChartFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAD.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DeptID> DeptIDs { get; set; }
    }
}
