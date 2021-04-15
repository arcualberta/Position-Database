using System;
using System.Collections.Generic;
using System.Text;

namespace PositionDatabase.Shared
{
    public class Position
    {
        public enum eContractType { FullTime, PartTime }

        public Guid Id { get; set; }
        public string PositionNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
        public decimal Workload { get; set; }
        public eContractType ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<SalaryComponent> SalaryComponents { get; set; }

        public Guid PersonId { get; set; }
        public Person Person { get; set; }
    }
}
