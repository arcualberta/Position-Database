using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PD.Data;
using PD.Models;
using PD.Services;

namespace PD.Controllers
{
    /// <summary>
    /// Allows viewing and editing position assignment details.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class PositionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;
        //private readonly IPdDataProtector _dataProtector;
        public PositionsController(ApplicationDbContext context, DataService dataService/*, IPdDataProtector dataProtector*/)
        {
            _context = context;
            _dataService = dataService;
            //_dataProtector = dataProtector;
        }

        /// <summary>
        /// Displays details of a position assignment with a given ID.
        /// </summary>
        /// <param name="id">PositionAssignment ID.</param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            PositionAssignment pa = _dataService.GetPositionAssignment(id);

            ViewBag.DataProtector = _dataService._dataProtector;

            return View(pa);
        }
    }
}