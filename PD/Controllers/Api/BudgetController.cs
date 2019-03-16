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
        public async Task<ActionResult<BudgetSummary[]>> Index(DateTime from, DateTime to, int stepInMonths = 1, int? deptId = null)
        {
            int numDataPoints = (int) Math.Ceiling((double) ((to - from).Days / (stepInMonths * 30)));
            List<BudgetSummary> ret = new List<BudgetSummary>(numDataPoints);
            for (DateTime t = from; t <= to; t = t.AddMonths(stepInMonths))
            {
                var compensations = _context.Compensations
                    .Include(comp => comp.PositionAssignment)
                    .Where(comp => t >= comp.StartDate
                                   && t <= comp.EndDate
                                   && (comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.Active
                                      || comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.PostRetirement
                                      || comp.PositionAssignment.Status == Models.PositionAssignment.eStatus.PreRetirement
                                      )
                          );

                if (deptId.HasValue)
                    compensations = compensations.Where(comp => comp.PositionAssignment.Position.PrimaryDepartmentId == deptId);
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
    }
}