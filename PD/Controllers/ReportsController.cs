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
        private readonly DataService _dataService;
        //private readonly IPdDataProtector _dataProtector;
        public ReportsController(ApplicationDbContext context, DataService dataService/*, IPdDataProtector dataProtector*/)
        {
            _context = context;
            _dataService = dataService;
            //_dataProtector = dataProtector;
        }

        public IActionResult Positions(PositionFilter filter)
        {
            if (filter == null)
                filter = new PositionFilter();

            ReportService srv = new ReportService(_context);
            IQueryable<PositionAssignment> positionAssignments = srv.GetPositionAssignments(filter);

            ViewBag.Filter = filter;
            ViewBag.DataProtector = _dataService._dataProtector;

            return View("PositionAssignments", positionAssignments);
            //return View(positionAssignments.ToList().Select(pp => new PositionViewModel(pp, filter.Date, srv.DataProtector)));
        }

        public IActionResult Details(int id, DateTime targetDate)
        {
            ReportService srv = new ReportService(_context);
            PositionAssignment pa = srv.GetPositionAssignment(id, targetDate, true, true, true, true);

            ViewBag.Filter = new PositionFilter() { Date = targetDate };
            ViewBag.DataProtector = _dataService._dataProtector;

            return View(pa);
        }
    }
}

