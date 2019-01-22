using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Services;

namespace PD.Controllers
{
    public class UploadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtectionProvider _provider;
        private readonly IConfiguration _configuration;

        public UploadController(ApplicationDbContext context, IDataProtectionProvider provider, IConfiguration configuration)
        {
            _context = context;
            _provider = provider;
            _configuration = configuration;
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

            if (files.Count() > 0)
            {
                var formFile = files[0];
                if (formFile.Length > 0)
                {
                    string tmpFile = Path.Combine(path, filenamePrefix).Replace('.', '_') + "_" + formFile.FileName;
                    using (var stream = new FileStream(tmpFile, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    ////BackgroundService srv = new BackgroundService(_context);
                    ////string jobKey = "Faculty Data import";
                    ////string jobId = srv.Enque(() => 
                    ////    new ImportService(new BackgroundService(_configuration).CreateDbContext())
                    ////        .InjestFacultySalaryAdjustmentData(tmpFile, worksheetName, new DateTime(2015, 7, 1).Date,
                    ////            new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date, true),
                    ////    jobKey);

                    ////if(string.IsNullOrEmpty(jobId))
                    ////{
                    ////    ViewBag.Message = "Previous job is still running. No new job added.";
                    ////    ViewBag.MessageClass = "alert-danger";
                    ////    System.IO.File.Delete(tmpFile);
                    ////}
                    ////else
                    ////{
                    ////    ViewBag.Message = string.Format("New job Added. Job ID: {0}", jobId);
                    ////    ViewBag.MessageClass = "alert-info";
                    ////}

                    ViewBag.FilePath = tmpFile;


                    //processing the uploaded file
                    ImportService ds = new ImportService(_context);
                    ds.InjestFacultySalaryAdjustmentData(tmpFile, worksheetName, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date);

                    //Deleting the temporary file
                    System.IO.File.Delete(tmpFile);
                }
            }

            return View();
            //return Ok(new { count = files.Count, size });
        }
    }
}