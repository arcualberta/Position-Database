using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Models;
using PD.Models.AppViewModels;
using PD.Models.AppViewModels.Filters;
using PD.Services;

namespace PD.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ReportService _reportService;
        public ReportsController(ApplicationDbContext context, ReportService reportService/*, IPdDataProtector dataProtector*/)
        {
            _context = context;
            _reportService = reportService;
        }

        public IActionResult People(/*PositionFilter filter*/)
        {
            //if (filter == null)
            //    filter = new PositionFilter();

            //IQueryable<PositionAssignment> positionAssignments = _reportService.GetPositionAssignments(filter);

            //ViewBag.Filter = filter;
            //ViewBag.DataProtector = _reportService._dataProtector;

            return View();
        }

        public IActionResult Positions(PositionFilter filter)
        {
            if (filter == null)
                filter = new PositionFilter();

            IQueryable<PositionAssignment> positionAssignments = _reportService.GetPositionAssignments(filter);

            ViewBag.Filter = filter;
            ViewBag.DataProtector = _reportService._dataProtector;

            return View("PositionAssignments", positionAssignments);
            //return View(positionAssignments.ToList().Select(pp => new PositionViewModel(pp, filter.Date, srv.DataProtector)));
        }

        public IActionResult Details(int id, DateTime targetDate)
        {
            PositionAssignment pa = _reportService.GetPositionAssignment(id, targetDate, true, true, true, true);

            ViewBag.Filter = new PositionFilter() { Date = targetDate };
            ViewBag.DataProtector = _reportService._dataProtector;

            return View(pa);
        }
    }
}

