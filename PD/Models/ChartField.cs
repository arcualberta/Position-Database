using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PD.Models
{
    /// <summary>
    /// ChartField
    /// Represents a field in a ChartString
    /// </summary>
    public class ChartField
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ChartField2ChartStringJoin> ChartStrings { get; } = new List<ChartField2ChartStringJoin>();
    }
}
