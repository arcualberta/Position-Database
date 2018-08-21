using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeId { get; set; }
        public DateTime? BirthDate { get; set; }

        public virtual ICollection<PersonPosition> PersonPositions { get; set; } = new List<PersonPosition>();

    }
}
