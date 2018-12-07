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
        /// <summary>
        /// Gets the data protector. Used for encryption and decryption of data fields.
        /// </summary>
        /// <value>
        /// The data protector.
        /// </value>
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

        public PositionAssignment GetPositionAssignment(
            int id, DateTime sampleDate,
            bool includeCompensations = false,
            bool includeChangeLog = false,
            bool includePosition = false,
            bool includePerson = false)
        {
            IQueryable<PositionAssignment> matches = Db.PositionAssignments;

            if (includeCompensations)
                matches = matches.Include(x => x.Compensations);

            if (includeChangeLog)
                matches = matches.Include(x => x.ChangeLog);

            if (includePosition)
                matches = matches.Include(x => x.Position);

            matches = matches.Where(x => x.StartDate <= sampleDate && (!x.EndDate.HasValue || x.EndDate >= sampleDate));

            return matches.FirstOrDefault();
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
                    .Include(a => a.Person);
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
                .Include(pa => pa.Position)
                .Include(pa => pa.Person)
                .Where(pa =>
                    (!pa.StartDate.HasValue || pa.StartDate.Value <= filter.Date)
                    && (!pa.EndDate.HasValue || pa.EndDate.Value > filter.Date)
                    );

            return associations;
        }
    }
}
