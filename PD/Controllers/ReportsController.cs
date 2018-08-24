﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            List<PersonPosition> positionAssignments = srv.GetPersonPositionAssociations(filter.PositionType, filter.Date, filter.IsActive).ToList();

            ViewBag.Filter = filter;
            return View(positionAssignments.ToList().Select(pp => new PositionViewModel(pp)));
        }
    }
}