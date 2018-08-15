using PD.Models.ChartFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class SalaryViewModel
    {
        public string FiscalYear { get; set; }
        public decimal LastSalary { get; set; }
        public decimal CurrentSalary { get; set; }
        public string BargUnit { get; set; }
        public Speedcode Speedcode { get; set; }
        public DeptID DeptId { get; set; }
        public Fund Fund { get; set; }
        public Program Program { get; set; }
        public Account Account { get; set; }
    }
}
