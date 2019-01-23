using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Services;

namespace PD.Controllers
{
    public class ProjectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataService _dataService;

        public ProjectionsController(ApplicationDbContext context, DataService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        public void test()
        {

        }

        public IActionResult Index(int? p = null)
        {
            BackgroundService srv = new BackgroundService(_context);

            string jobKey = p.HasValue && p.Value > 0 ? null : "test-job";
            string jobId = srv.Enque(() => _dataService.TestService(60), jobKey);

            ViewBag.Message = string.IsNullOrEmpty(jobId) ? "Previous job is still running. No new job added." : string.Format("New job Added. Job ID: {0}", jobId);

            ////var jobInfo = JobStorage.Current.GetMonitoringApi().JobDetails("7");

            ////var jobId = BackgroundJob.Enqueue(() => srv.TestService(60));

            ////srv.Enque(() =>
            ////{
            ////    ApplicationDbContext context = new ApplicationDbContext();
            ////    DataService srv = new DataService(new ApplicationDbContext());
            ////    srv.TestService(60);
            ////}, "key");

            ////ViewBag.JobId = jobId;

            return View();
        }
    }
}