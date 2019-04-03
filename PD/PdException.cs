using PD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public class PdException : Exception
    {
        public DateTime SalaryCycleStartDate { get; set; }
        public DateTime SalaryCycleEndDate { get; set; }

        public PdException(string message, DateTime salaryCycleStartDate, DateTime salaryCycleEndDate)
            : base(message)
        {
            SalaryCycleStartDate = salaryCycleStartDate;
            SalaryCycleEndDate = salaryCycleEndDate;
        }

        public PdException(string message, PositionAssignment pa, DateTime targetDate)
            : base(message)
        {
            SalaryCycleStartDate = pa.GetSalaryCycleStartDate(targetDate);
            SalaryCycleEndDate = pa.GetSalaryCycleEndDate(targetDate);
        }
    }
}
