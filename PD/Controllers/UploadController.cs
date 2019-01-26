using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
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
        private readonly ImportService _importService;
        private readonly IPdDataProtector _dataProtector;

        public UploadController(ApplicationDbContext context, ImportService importService, IPdDataProtector dataProtector)
        {
            _context = context;
            _importService = importService;
            _dataProtector = dataProtector;
        }

        [HttpGet]
        public IActionResult FacultySalaryAdjustmentData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FacultySalaryAdjustmentData(List<IFormFile> files)
        {
            bool encrypt = true;

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
                    string tmpFile = Path.Combine(path, filenamePrefix).Replace('.', '_') + "_" + formFile.FileName + ".dat";

                    if (encrypt)
                    {
                        using (var ms = new MemoryStream())
                        {
                            formFile.CopyTo(ms);
                            byte[] bytes = ms.ToArray();
                            string data = Convert.ToBase64String(bytes);
                            data = _dataProtector.Encrypt(data);
                            System.IO.File.WriteAllText(tmpFile, data);
                        }
                    }
                    else
                    {
                        using (var stream = new FileStream(tmpFile, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }

                    string jobKey = "Faculty Data import";
                    if (_importService.IsJobActive(jobKey))
                    {
                        System.IO.File.Delete(tmpFile);
                        ViewBag.Message = "Previous job is still running. No new job added.";
                        ViewBag.MessageClass = "alert-danger";
                    }
                    else
                    {
                        var jobId = BackgroundJob.Enqueue<ImportService>(srv => srv.InjestFacultySalaryAdjustmentData(
                            tmpFile,
                            encrypt,
                            worksheetName,
                            new DateTime(2015, 7, 1).Date,
                            new DateTime(2016, 06, 30).Date,
                            new DateTime(2017, 06, 30).Date,
                            true
                            ));

                        //Saving the job ID
                        var job = _context.HangFireJobs.Where(j => j.JobKey == jobKey).FirstOrDefault();
                        if (job == null)
                        {
                            job = new Models.HangFireJob() { JobId = jobId, JobKey = jobKey };
                            _context.HangFireJobs.Add(job);
                        }
                        else
                            job.JobId = jobId;
                        _context.SaveChanges();

                        ViewBag.Message = string.Format("New job Added. Job ID: {0}", jobId);
                        ViewBag.MessageClass = "alert-info";
                    }

                    ViewBag.FilePath = tmpFile;

                    ////////BackgroundService srv = new BackgroundService(_context);
                    ////////string jobId = srv.Enque(() =>
                    ////////    new ImportService(new BackgroundService(_configuration).CreateDbContext())
                    ////////        .InjestFacultySalaryAdjustmentData(tmpFile, worksheetName, new DateTime(2015, 7, 1).Date,
                    ////////            new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date, true),
                    ////////    jobKey);

                    ////////if (string.IsNullOrEmpty(jobId))
                    ////////{
                    ////////    ViewBag.Message = "Previous job is still running. No new job added.";
                    ////////    ViewBag.MessageClass = "alert-danger";
                    ////////    System.IO.File.Delete(tmpFile);
                    ////////}
                    ////////else
                    ////////{
                    ////////    ViewBag.Message = string.Format("New job Added. Job ID: {0}", jobId);
                    ////////    ViewBag.MessageClass = "alert-info";
                    ////////}

                    ViewBag.FilePath = tmpFile;


                    //////processing the uploaded file
                    ////_importService.InjestFacultySalaryAdjustmentData(tmpFile, worksheetName, new DateTime(2015, 7, 1).Date,
                    ////    new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date);

                    //////Deleting the temporary file
                    ////System.IO.File.Delete(tmpFile);
                }
            }

            return View();
            //return Ok(new { count = files.Count, size });
        }
    }
}