using PD.Data;
using PD.Models;
using PD.Models.Compensations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public abstract class ProjectionService : PdServiceBase
    {
        public ProjectionService(ApplicationDbContext db)
            : base(db)
        {
        }

        public virtual PositionAssignment ProjectSalary(
            int positionAssignmentId,
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision,
            decimal defaultContractSettlement,
            bool updateDatabase = false)
        {
            PositionAssignment pa = GetPositionAssignment(positionAssignmentId, targetDate, true, false, true, false);
            if (pa == null)
                return null;

            return ProjectSalary(pa, targetDate, cycleStartDate, cycleEndDate, defaultMeritDecision, updateDatabase);
        }

        public abstract PositionAssignment ProjectSalary(
            PositionAssignment pa,
            DateTime targetDate,
            DateTime cycleStartDate,
            DateTime cycleEndDate,
            decimal defaultMeritDecision,
            bool updateDatabase = false);

        public abstract void CalculateSalary(PositionAssignment pa, DateTime targetDate, bool projectionModeFlag);
    }
}
