using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models.AppViewModels
{
    public class PositionViewModel
    {
        public string Department { get; set; }

        [Display(Name = "Position #")]
        public string PositionNumber { get; set; }

        [Display(Name = "Title")]
        public string PositionTitle { get; set; }

        [Display(Name = "Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; }

        [Display(Name = "FT/PT")]
        public Position.ePositionWorkload WorkLoad { get; set; }

        [Display(Name = "Status")]
        public PersonPosition.eStatus Status { get; set; }

        [Display(Name = "Contract")]
        public Position.ePositionContract ContractType { get; set; }

        public Compensation Compensation { get; set; }



        public PositionViewModel()
        {

        }

        public PositionViewModel(PersonPosition pp, string fiscalYear, DataProtector dp)
        {
            Department = "";
            PositionNumber = pp.Position.Number;
            PositionTitle = pp.Position.Title;
            EmployeeName = dp.Decrypt(pp.Person.Name);
            EmployeeId = pp.Person.EmployeeId;
            WorkLoad = pp.Position.PositionWorkload;
            Status = pp.Status;
            ContractType = pp.ContractType;
            Compensation = pp.Compensations.Where(c => c.Year == fiscalYear).FirstOrDefault();

        }
    }
}
