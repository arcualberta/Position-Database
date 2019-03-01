using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PD.Data;
using PD.Services;

namespace PD.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;
        public EmployeesController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        public IActionResult Index(string emplyeeId, int? departmentId)
        {

            return View();
        }
    }
}