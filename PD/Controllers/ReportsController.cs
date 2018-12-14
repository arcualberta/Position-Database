using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Models;
using PD.Models.AppViewModels;
using PD.Models.AppViewModels.Filters;
using PD.Services;

namespace PD.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Positions(PositionFilter filter)
        {
            if (filter == null)
                filter = new PositionFilter();

            ReportService srv = new ReportService(_context);
            IQueryable<PositionAssignment> positionAssignments = srv.GetPositionAssignments(filter);

            ViewBag.Filter = filter;
            ViewBag.DataProtector = srv.DataProtector;

            return View("PositionAssignments", positionAssignments);
            //return View(positionAssignments.ToList().Select(pp => new PositionViewModel(pp, filter.Date, srv.DataProtector)));
        }

        public IActionResult Details(int id, DateTime targetDate)
        {
            ReportService srv = new ReportService(_context);
            PositionAssignment pa = srv.GetPositionAssignment(id, targetDate, true, true, true, true);

            ViewBag.Filter = new PositionFilter() { Date = targetDate };
            ViewBag.DataProtector = srv.DataProtector;

            return View(pa);
        }
    }
}

