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
using Hangfire;

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

        /// <summary>
        /// Gets the position assignment with a given ID irrespective of the time.
        /// The returned object includes the associated objects such as person, position, compensations, etc.) 
        /// </summary>
        /// <param name="id">The unique ID (database primary key, NOT the position number)</param>
        /// <returns>PositionAssignment object, or null if no position assignment with the given ID exists.</returns>
        public PositionAssignment GetPositionAssignment(int id)
        {
            return GetPositionAssignment(id, null, true, true, true, true, true);
        }

        public PositionAssignment GetPositionAssignment(
            int id, 
            DateTime? sampleDate,
            bool includeCompensations = false,
            bool includeAuditTrail = false,
            bool includePosition = false,
            bool includePerson = false, 
            bool includePredecessorPositionAssignment = false)
        {
            IQueryable<PositionAssignment> matches = Db.PositionAssignments;

            if (includeCompensations)
                matches = matches.Include(x => x.Compensations);

            if (includeAuditTrail)
                matches = matches.Include(x => x.AuditTrail);

            if (includePosition)
                matches = matches.Include(x => x.Position);

            if (includePerson)
                matches = matches.Include(x => x.Person);

            if(includePredecessorPositionAssignment)
                matches = matches.Include(x => x.Predecessor);

            if (sampleDate.HasValue)
                matches = matches.Where(x => x.Id == id && x.StartDate <= sampleDate && (!x.EndDate.HasValue || x.EndDate >= sampleDate));

            return matches.FirstOrDefault();
        }

        public IQueryable<PositionAssignment> GetPositionAssignments(
            int? positionId = null,
            string positionNumber = null,
            DateTime? date = null,
            bool includePositionInfo = true,
            bool includePersonInfo = true)
        {
            if (!date.HasValue)
                date = DateTime.Now.Date;

            IQueryable<PositionAssignment> positionAssignments;

            if (includePersonInfo)
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
                .Include(pa => pa.Compensations)
                .Include(pa => pa.AuditTrail)
                .Where(pa =>
                    (!pa.StartDate.HasValue || pa.StartDate.Value <= filter.Date)
                    && (!pa.EndDate.HasValue || pa.EndDate.Value > filter.Date)
                    );

            return associations;
        }

        public bool IsJobActive(string jobKey)
        {
            var job = Db.HangFireJobs.Where(j => j.JobKey == jobKey).FirstOrDefault();
            if (job != null)
            {
                var jobInfo = JobStorage.Current.GetMonitoringApi().JobDetails(job.JobId);
                if (jobInfo != null && 
                    !(jobInfo.History[0].StateName == "Succeeded" || jobInfo.History[0].StateName == "Deleted"))
                    return true;
            }
            return false;
        }
    }
}
