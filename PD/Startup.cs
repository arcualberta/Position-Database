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

            services.AddDbContext<ApplicationDbContext>(options =>options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
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

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddDataProtection();
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

            //Checking admins
            var checkAdmin = userManager.GetUsersInRoleAsync("Admin");
            checkAdmin.Wait();

            if(checkAdmin.Result.Count() == 0)
            {
                //Creating default admin account if an admin account does not exist.
                string defaultAdminUserName = Configuration["Authentication:SystemAdmin:UserName"];
                if (string.IsNullOrEmpty(defaultAdminUserName))
                    throw new Exception("Default admin user not defined at Authentication:SystemAdmin:UserName in the applocation configuration.");

                string defaultAdminEmail = Configuration["Authentication:SystemAdmin:Email"];
                if (string.IsNullOrEmpty(defaultAdminEmail))
                    throw new Exception("Default admin email not defined at Authentication:SystemAdmin:Email in the applocation configuration.");

                string defaultAdminPassword = Configuration["Authentication:SystemAdmin:InitialPassword"];
                if (string.IsNullOrEmpty(defaultAdminPassword))
                    throw new Exception("Default admin password not defined at Authentication:SystemAdmin:InitialPassword in the applocation configuration.");

                var defaultAdmin = new IdentityUser()
                {
                    UserName = defaultAdminUserName,
                    Email = defaultAdminEmail
                };

                var task = userManager.CreateAsync(defaultAdmin, defaultAdminPassword);
                task.Wait();

                if (!task.Result.Succeeded)
                {
                    string error = string.Join(" ", task.Result.Errors.Select(err => err.Description));
                    throw new Exception("Failed to create default admin user. " + error);
                }

                task = userManager.AddToRoleAsync(defaultAdmin, "Admin");
                task.Wait();
                if (!task.Result.Succeeded)
                {
                    string error = string.Join(" ", task.Result.Errors.Select(err => err.Description));
                    throw new Exception("Failed to assign Admin role to default admin user. " + error);
                }
            }
        }
    }
}
