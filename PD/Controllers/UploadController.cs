using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PD.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult FacultySalaryAdjustmentData()
        {
            if(Request.Method == "POST")
            {
                ViewBag.UploadStatus = true;

            }
            return View();
        }
    }
}