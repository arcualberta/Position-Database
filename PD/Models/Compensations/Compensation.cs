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
        /// <summary>
        /// Gets or sets the compensation value calculated in dollars.
        /// </summary>
        /// <value>
        /// The compensation value calculated in dollars.
        /// </value>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the manually-overriden value of compensation in dollars, if any.
        /// </summary>
        /// <value>
        /// The manually-overriden value of compensation in dollars, if any.
        /// </value>
        [Display(Name = "Manual Override Value:")]
        public decimal? ManualOverride { get; set; }

        /// <summary>
        /// Gets or sets the start date for the compensation.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the compensation.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is projection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is projection; otherwise, <c>false</c>.
        /// </value>
        [Display(Name = "Is Projection?")]
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
