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
        private readonly IConfiguration _configuration;
        public ProjectionsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void test()
        {

        }

        public IActionResult Index(int? p = null)
        {
            BackgroundService srv = new BackgroundService(_context);

            string jobKey = p.HasValue && p.Value > 0 ? null : "test-job";
            string jobId = srv.Enque(() => new DataService(new BackgroundService(_configuration).CreateDbContext()).TestService(60), jobKey);

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