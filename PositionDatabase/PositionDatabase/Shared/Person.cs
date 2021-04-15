using System;
using System.Collections.Generic;
using System.Text;

namespace PositionDatabase.Shared
{
    public class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Position> Positions { get; set; }

        public Person()
        {
            Id = Guid.NewGuid();
            Positions = new List<Position>();
        }
    }
}
