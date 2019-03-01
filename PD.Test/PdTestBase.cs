using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Services;
using PD.Services.Projections;

namespace PD.Test
{
    public class PdTestBase
    {
        public readonly ServiceProvider _serviceProvider;

        public PdTestBase()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            //Creating a service collection
            var services = new ServiceCollection();

            //Registering application DB Context
            string dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(dbConnectionString)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) //Disables client evaluation
                );


            #region Data Protection Registration
            //Creating a separate database context that is connected to a separate database for storing
            //encryption keys and adding it to the service collection
            string dataProtectionDbConnectionString = configuration.GetConnectionString("DataProtectionConnection");
            services.AddDbContext<DataProtectionDbContext>(options => options
                .UseSqlServer(dataProtectionDbConnectionString)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) //Disables client evaluation
                );

            //Adding a data protection service which is tied to the above SQL database into the service collection
            services.AddDataProtection()
                .SetApplicationName("ARC.PositionDatabase")
                .PersistKeysToDbContext<DataProtectionDbContext>();

            services.AddScoped<IPdDataProtector, PdDataProtector>();
            #endregion

            #region Service Class Registration
            services.AddScoped<DataService, DataService>();
            services.AddScoped<ImportService, ImportService>();
            services.AddScoped<FacultyProjectionService, FacultyProjectionService>();

            #endregion


            //Creating a service provider and assigning it to the member variable so that it can be used by 
            //test methods.
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
