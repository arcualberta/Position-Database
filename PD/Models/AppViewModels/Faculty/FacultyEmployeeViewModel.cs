using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels.Faculty
{
    public class FacultyEmployeeViewModel : EmployeeViewModel
    {
        public FacultyEmployeeViewModel()
        {
            Salary = new FacultySalaryViewModel();
        }

        public string Rank { get; set; }

        public FacultySalaryViewModel FacultySalary { get { return Salary as FacultySalaryViewModel; } }
    }
}
