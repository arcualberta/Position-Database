using PD.Models.Compensations;
using PD.Models.Positions;
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
        public decimal WorkLoad { get; set; }

        [Display(Name = "Status")]
        public PositionAssignment.eStatus Status { get; set; }

        [Display(Name = "Contract")]
        public Position.eContractType ContractType { get; set; }

        public Compensation Compensation { get; set; }



        public PositionViewModel()
        {

        }

        public PositionViewModel(PositionAssignment pa, DateTime sampleDate, DataProtector dp)
        {
            Department = "";
            PositionNumber = pa.Position.Number;
            PositionTitle = pa.Position.Title;
            EmployeeName = dp.Decrypt(pa.Person.Name);
            EmployeeId = pa.Person.EmployeeId;
            WorkLoad = pa.Position.Workload;
            Status = pa.Status;
            ContractType = pa.Position.ContractType;
            Compensation = pa.Compensations.Where(c => c.StartDate <= sampleDate && sampleDate <= c.EndDate).FirstOrDefault();

        }
    }
}
