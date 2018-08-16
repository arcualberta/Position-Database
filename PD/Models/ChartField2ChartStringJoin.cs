﻿namespace PD.Models
{
    /// <summary>
    /// ChartField2ChartStringJoin
    /// The join table which implements the many-to-many relationship between the
    /// ChartField entity and ChatString entity.
    /// </summary>
    public class ChartField2ChartStringJoin
    {
        public int ChartStringId { get; set; }
        public virtual ChartString ChartString { get; set; }
        public int ChartFieldId { get; set; }
        public virtual ChartField ChartField { get; set; }
    }
}
