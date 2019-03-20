using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PD.Data;
using PD.Models.AppViewModels;
using PD.Services.Projections;
using Microsoft.AspNetCore.Authorization;
using PD.Models.Compensations;

namespace PD.Controllers.Api
{
    [Authorize(Roles = "Admin,Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompensationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FacultyProjectionService _facultyProjectionService;

        public CompensationsController(ApplicationDbContext context, FacultyProjectionService facultyProjectionService)
        {
            _context = context;
            _facultyProjectionService = facultyProjectionService;
        }

        [HttpPost("compute/{positionAssignmentId}/{compensationStartDate}/{compensationEndDate}")]
        public async Task<ActionResult<ComputationResult>> Compute(int positionAssignmentId, DateTime? compensationStartDate, DateTime? compensationEndDate)
        {
            ComputationResult result = new ComputationResult();

            bool updated = _facultyProjectionService.ComputeCompensation(positionAssignmentId, compensationStartDate.Value, result);
            if (updated)
            {
                var positionAssignment = await _context.PositionAssignments
                  .Where(pa => pa.Id == positionAssignmentId)
                  .Include(pa => pa.Compensations)
                  .Include(pa => pa.AuditTrail)
                  .FirstOrDefaultAsync();

                Compensation[] data = positionAssignment.Compensations
                    .Where(c => c.StartDate == compensationStartDate
                                && (c.EndDate.HasValue == false || c.EndDate == compensationEndDate))
                    .ToArray();

                //Eliminating unnecessary data for the view
                foreach(var d in data)
                    d.PositionAssignment = null;

                result.Data = data;
            }

            //Removing duplicate error messages
            result.Errors = result.Errors.Distinct().ToList();
            return result;
        }
    }
}