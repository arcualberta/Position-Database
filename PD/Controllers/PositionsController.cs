using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PD.Controllers
{
    public class PositionsController : Controller
    {
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}