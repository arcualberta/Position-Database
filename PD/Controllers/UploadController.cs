using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PD.Data;
using PD.Services;

namespace PD.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult FacultySalaryAdjustmentData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FacultySalaryAdjustmentData(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            string worksheetName = "ARC Academic Salary Adj2015 16";

            // full path to file in temp location
            string path = Path.GetTempPath();
            string filenamePrefix = Path.GetRandomFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string tmpFile = Path.Combine(path, filenamePrefix).Replace('.', '_') + "_" + formFile.FileName;
                    using (var stream = new FileStream(tmpFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    //processing the uploaded file
                    DataService ds = new DataService(_context);
                    ds.InjestFacultySalaryAdjustmentData(tmpFile, worksheetName, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016,06,30).Date, new DateTime(2015, 07, 02).Date, new DateTime(2017, 06, 30).Date);

                    //Deleting the temporary file
                    System.IO.File.Delete(tmpFile);
                }
            }

            return View();
            //return Ok(new { count = files.Count, size });
        }
    }
}