using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PD.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using PD.Services;
using PD.Services.Projections;

namespace PD
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            string dbConnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(dbConnectionString)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) //Disables client evaluation
                );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            ////services.AddSingleton<IConfiguration>(Configuration);

            #region Data Protection Registration
            //Data protection registration (with SQL key storage)
            //==================================================
            //Reference: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-2.2&tabs=visual-studio

            //Creating a separate database context for storing encryption keys and adding it to the service collection
            string dataProtectionDbConnectionString = Configuration.GetConnectionString("DataProtectionConnection");
            services.AddDbContext<DataProtectionDbContext>(options => options
                .UseSqlServer(dataProtectionDbConnectionString)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) //Disables client evaluation
                );

            //Adding a data protection service which is tied to the above SQL database into the service collection
            //Reference: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-2.2
            services.AddDataProtection()
                .SetApplicationName("ARC.PositionDatabase") //If we don't specify the app name then keys are tied to where the application is running from on the file system.
                .PersistKeysToDbContext<DataProtectionDbContext>();

            //Registering a custom-defined data protector interface along with an implementation class of it 
            //with the service collection. Note that the concrete implementation of the PdDataProtector class
            //takes an interface IDataProtectionProvider as an input argument of the constructor. The dependency
            //injection creates an instance of this interface using the data protection added above into the service 
            //collection and passes it on to the constructor when it instantiate the PdDataProtector class.
            services.AddScoped<IPdDataProtector, PdDataProtector>();

            #endregion

            #region Service Class Registration
            services.AddScoped<DataService, DataService>();
            services.AddScoped<ImportService, ImportService>();
            services.AddScoped<ReportService, ReportService>();
            services.AddScoped<FacultyProjectionService, FacultyProjectionService>();
            #endregion

            //HangFire background job processing
            services.AddHangfire(x => x.UseSqlServerStorage(dbConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            //Initializing custom roles 
            string[] roleNames = { "Admin", "Manager", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName);
                roleExist.Wait();
                if (!roleExist.Result)
                {
                    //create the roles and seed them to the database: Question 1
                    var task = roleManager.CreateAsync(new IdentityRole(roleName));
                    task.Wait();
                }
            }

            //Starting Hangfire background processing server
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", options: new DashboardOptions { Authorization = new[] { new HangFireAuthorizationFilter() } });

        }
    }
}
