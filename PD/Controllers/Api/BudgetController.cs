using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PD.Data;
using PD.Models.AppViewModels;
using Microsoft.EntityFrameworkCore;
using PD.Models.Compensations;
using PD.Models;

namespace PD.Controllers.Api
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/People/5
        [HttpGet]
        public async Task<ActionResult<BudgetSummary[]>> Index(DateTime from, DateTime to, int stepInMonths = 1, string budgetOptions = null, string positionTypes = null, int deptId = 0)
        {
            int numDataPoints = (int)Math.Ceiling((double)((to - from).Days / (stepInMonths * 30)));

            List<string> requestedBudgetOptions = budgetOptions == null
                ? new List<string>()
                : budgetOptions.Split(",").ToList();

            List<string> requestedPositionTypes = positionTypes == null
                ? new List<string>()
                : positionTypes.Split(",").ToList();

            List<BudgetSummary> ret = new List<BudgetSummary>(numDataPoints);
            for (DateTime t = from; t <= to; t = t.AddMonths(stepInMonths))
            {
                var compensations = _context.Compensations
                    .Include(comp => comp.PositionAssignment)
                    .Where(comp => t >= comp.StartDate
                                   && (comp.EndDate.HasValue == false || t <= comp.EndDate)
                                   && (comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.Active
                                      || comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.PostRetirement
                                      || comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.PreRetirement
                                      )
                          );

                if (deptId > 0)
                    compensations = compensations.Where(comp => comp.PositionAssignment.Position.PrimaryDepartmentId == deptId);

                bool includeBaseSalary = requestedBudgetOptions.Contains("Base Salary");
                compensations = compensations.Where(comp =>
                    requestedBudgetOptions.Contains(comp.Name) ||
                    includeBaseSalary && comp is Salary
                    );
                compensations = compensations.Where(comp => requestedPositionTypes.Contains(comp.PositionAssignment.Position.Title));

                var x = compensations.ToList();
                decimal budget = await compensations.Select(c => c.Value).SumAsync();

                int positionCount = await compensations.Select(c => c.PositionAssignment.Id).Distinct().CountAsync();
                BudgetSummary budgetSummary = new BudgetSummary()
                {
                    Date = t,
                    Budget = budget > 0 ? budget : (decimal?)null,
                    PositionCount = positionCount > 0 ? positionCount : (int?)null
                };

                ret.Add(budgetSummary);
            }

            return ret.ToArray();
        }

        [HttpGet("alerts/{from}/{to}/{deptId}/{positionTypes}")]
        public async Task<ActionResult<string[]>> GetAlerts(DateTime from, DateTime to, int deptId = 0, string positionTypes = null)
        {
            List<string> requestedPositionTypes = positionTypes == null
                ? new List<string>()
                : positionTypes.Split(",").ToList();

            //Getting all warnings and errors that occurred during salary computations over
            //the requested time period
            var query = _context.AuditTrail
                .Include(au => au.PositionAssignment.Position)
                .Where(au =>
                    requestedPositionTypes.Contains(au.PositionAssignment.Position.Title)
                    && au.IsHistoric == false
                    && (au.AuditType == AuditRecord.eAuditRecordType.Error || au.AuditType == AuditRecord.eAuditRecordType.Warning)
                    && (au.SalaryCycleStartDate >= from && au.SalaryCycleStartDate <= to || au.SalaryCycleEndDate >= from && au.SalaryCycleEndDate <= to)
                    );

            if (deptId > 0)
                query = query.Where(au => au.PositionAssignment.Position.PrimaryDepartmentId == deptId);

            string[] alerts = await query.Select(au => au.Message).Distinct().ToArrayAsync();

            return alerts;
        }
     }
}