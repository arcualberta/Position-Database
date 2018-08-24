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

        public IQueryable<PersonPosition> GetPersonPositionAssociations(Position.ePositionType positionType, DateTime? date, bool isActive = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<PersonPosition> associations = Db.PersonPositions
                .Include(pp => pp.Position)
                .Include(pp => pp.Person)
                .Where(pp =>
                    pp.Position.PositionType == positionType
                    && (!pp.StartDate.HasValue || pp.StartDate.Value <= date)
                    && (!pp.EndDate.HasValue || pp.EndDate.Value > date)
                    );

            return associations;
        }
        ////public IQueryable<Position> GetPositions(Position.ePositionType positionType, DateTime? date = null, bool isActive = true)
        ////{
        ////    if (!date.HasValue)
        ////        date = DateTime.Now.Date;

        ////    IQueryable<Position> positions = Db.Positions
        ////        .Where(pos =>
        ////            pos.PositionType == positionType
        ////            && pos.IsActive == isActive
        ////            && (!pos.StartDate.HasValue || pos.StartDate.Value <= date)
        ////            && (!pos.EndDate.HasValue || pos.EndDate.Value > date)
        ////            );

        ////    return positions;
        ////}

    }
}
