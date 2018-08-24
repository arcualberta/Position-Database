using PD.Data;
using PD.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PD.Services
{
    /// <summary>
    /// PdServiceBase: base class for all PD service classes
    /// </summary>
    public class PdServiceBase
    {
        public ApplicationDbContext Db { get; }

        public PdServiceBase(ApplicationDbContext db)
        {
            Db = db;
        }

        public IQueryable<Position> GetPositions(Position.ePositionType positionType, DateTime? date = null, bool isActive = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<Position> positions = Db.Positions
                .Where(pos =>
                    pos.PositionType == positionType
                    && pos.IsActive == isActive
                    && (!pos.StartDate.HasValue || pos.StartDate.Value <= date)
                    && (!pos.EndDate.HasValue || pos.EndDate.Value > date)
                    );

            return positions;
        }

        public IQueryable<PersonPosition> GetPersonPositionAssociations(Position.ePositionType positionType, DateTime? date, bool isActive = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<PersonPosition> associations = Db.PersonPositions;
                ////.Where(pp =>
                ////    pp.Position.PositionType == positionType);
                    ////&& (!pp.StartDate.HasValue || pp.StartDate.Value <= date)
                    ////&& (!pp.EndDate.HasValue || pp.EndDate.Value > date)
                    ////);

            return associations;
        }


        public IQueryable<PersonPosition> GetPersonPositionAssociations2(Position.ePositionType positionType, DateTime? date, bool isActive = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<Position> positions = GetPositions(positionType, date, isActive);

            IQueryable<PersonPosition> associations = positions.SelectMany(pos => pos.PersonPositions)
                .Where(pp =>
                    (!pp.StartDate.HasValue || pp.StartDate.Value <= date)
                    && (!pp.EndDate.HasValue || pp.EndDate.Value > date));

            return associations;
        }

        public IQueryable<Person> GetEmployees(Position.ePositionType positionType, DateTime? date, bool isActive = true)
        {
            IQueryable<PersonPosition> associations = GetPersonPositionAssociations(positionType, date, isActive);
            IQueryable<Person> employees = associations.Select(a => a.Person);
            return employees;
        }

    }
}
