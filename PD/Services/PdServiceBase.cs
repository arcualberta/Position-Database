using PD.Data;
using PD.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PD.Models.AppViewModels.Filters;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using PD.Models.Positions;

namespace PD.Services
{
    /// <summary>
    /// PdServiceBase: base class for all PD service classes
    /// </summary>
    public class PdServiceBase
    {
        public ApplicationDbContext Db { get; }

        private DataProtector _DataProtector;
        public DataProtector DataProtector
        {
            get
            {
                if (_DataProtector == null)
                {
                    var serviceCollection = new ServiceCollection();
                    serviceCollection.AddDataProtection();
                    var services = serviceCollection.BuildServiceProvider();

                    _DataProtector = ActivatorUtilities.CreateInstance<DataProtector>(services);
                }

                return _DataProtector;
            }
        }

        public PdServiceBase(ApplicationDbContext db)
        {
            Db = db;
        }

        public IQueryable<PositionAssignment> GetPositionAssignments(
            int? positionId = null,
            string positionNumber = null,
            DateTime? date = null,
            bool includePositionInfo = true,
            bool includePersinInfo = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<PositionAssignment> positionAssignments;

            if (includePersinInfo)
                positionAssignments = Db.PositionAssignments
                    .Include(a => a.Position)
                    .Include(a => a.Position.Person);
            else if (includePositionInfo)
                positionAssignments = Db.PositionAssignments.Include(a => a.Position);
            else
                positionAssignments = Db.PositionAssignments;

            positionAssignments = positionAssignments.Where(a => 
                (!a.StartDate.HasValue || a.StartDate.Value <= date)
                && (!a.EndDate.HasValue || a.EndDate.Value >= date));

            if (positionId.HasValue)
                positionAssignments = positionAssignments.Where(a => a.PositionId == positionId);

            if (!string.IsNullOrEmpty(positionNumber))
                positionAssignments = positionAssignments.Where(a => a.Position.Number == positionNumber);

            return positionAssignments;
        }

       public IQueryable<PositionAssignment> GetPositionAssignments(PositionFilter filter)
        {
            IQueryable<PositionAssignment> associations = Db.PositionAssignments
                .Include(pp => pp.Position)
                .Include(pp => pp.Position.Person)
                .Where(pp =>
                    (!pp.StartDate.HasValue || pp.StartDate.Value <= filter.Date)
                    && (!pp.EndDate.HasValue || pp.EndDate.Value > filter.Date)
                    );

            return associations;
        }

        //public Position GetPosition(string positionId, )

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
