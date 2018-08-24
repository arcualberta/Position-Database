using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class PositionViewModel
    {
        public string Department { get; set; }
        public string PositionNumber { get; set; }
        public string PositionTitle { get; set; }
        public string EmployeeName { get; set; } 
        public string EmployeeId { get; set; }

        public PositionViewModel()
        {

        }

        public PositionViewModel(PersonPosition pp)
        {
            Department = "";
            PositionNumber = pp.Position.Number;
            PositionTitle = pp.Position.Title;
            EmployeeName = pp.Person.Name;
            EmployeeId = pp.Person.EmployeeId;
        }
    }
}
