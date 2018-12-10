using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services.Projections.Rules
{
    public abstract class AbstractProjectionRule
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ApplicationDbContext Db { get; set; }

        public abstract bool Execute(ref PositionAssignment pa, DateTime targetDate);

        public AbstractProjectionRule(ApplicationDbContext db, string name, string description)
        {
            Name = name;
            Description = description;
            Db = db;
        }

        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            return Db.SalaryScales
                .Where(sc => sc.StartDate <= targetDate && sc.EndDate >= targetDate && sc.Name == positionTitle)
                .FirstOrDefault();
        }

    }
}
