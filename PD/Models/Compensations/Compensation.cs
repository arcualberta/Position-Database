using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace PD.Models.Compensations
{
    public abstract class Compensation
    {
        [Key]
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsProjection { get; set; }
        public string Notes { get; set; }

        public int PositionAssignmentId { get; set; }
        public virtual PositionAssignment PositionAssignment { get; set; }

        protected T Clone<T>() where T : Compensation
        {
            var type = typeof(T);
            T clone = Activator.CreateInstance(type) as T;
            clone.Value = Value;
            clone.StartDate = StartDate;
            clone.EndDate = EndDate;
            clone.IsProjection = IsProjection;
            clone.Notes = Notes;
            clone.PositionAssignment = PositionAssignment;
            return clone;
        }


        public abstract Compensation Clone();
     }
}
