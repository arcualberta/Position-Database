using Hangfire;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using PD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PD.Services
{
    public class BackgroundService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public BackgroundService(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public BackgroundService(IConfiguration config)
        {
            _config = config;
        }

        public ApplicationDbContext CreateDbContext()
        {
            var config = _config; // new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"));

            ApplicationDbContext db = new ApplicationDbContext(builder.Options);
            return db;
        }

        public string Enque(Expression<Action> action, string jobKey)
        {
            if (string.IsNullOrEmpty(jobKey))
                return BackgroundJob.Enqueue(action); // No need to ensure only one instance of this job is queued and executed at a time

            //Need to make sure only one instance of this job is queued and executed at a given time.

            //Check if ther are any active or enqueued jobs with the given name
            var job = _context.HangFireJobs.Where(j => j.JobKey == jobKey).FirstOrDefault();
            if(job != null)
            {
                var jobInfo = JobStorage.Current.GetMonitoringApi().JobDetails(job.JobId);
                if (jobInfo != null && jobInfo.History[0].StateName != "Succeeded")
                    return null;
            }

            //Either this job was never run before or the previous run is completed, so we can run it now.
            var jobId = BackgroundJob.Enqueue(action);

            //Saving the job ID
            if (job == null)
            {
                job = new Models.HangFireJob() { JobId = jobId, JobKey = jobKey };
                _context.HangFireJobs.Add(job);
            }
            else
                job.JobId = jobId;

            _context.SaveChanges();

            return jobId;
        }
    }
}
