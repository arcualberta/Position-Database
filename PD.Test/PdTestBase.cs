using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;

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

            //Creating a separate database context that is connected to a separate database for storing
            //encryption keys and adding it to the service collection
            string dataProtectionDbConnectionString = configuration.GetConnectionString("DataProtectionConnection");
            services.AddDbContext<DataProtectionDbContext>(options => options
                .UseSqlServer(dataProtectionDbConnectionString)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)) //Disables client evaluation
                );

            //Adding a data protection service which is tied to the above SQL database into the service collection
            services.AddDataProtection().PersistKeysToDbContext<DataProtectionDbContext>();

            //Registering a custom-defined data protector interface along with an implementation class of it 
            //with the service collection. Note that the concrete implementation of the PdDataProtector class
            //takes an interface IDataProtectionProvider as an input argument of the constructor. The dependency
            //injection creates an instance of this interface using the data protection added above into the service 
            //collection and passes it on to the constructor when it instantiate the PdDataProtector class.
            services.AddTransient<IPdDataProtector, PdDataProtector>();

            //Creating a service provider and assigning it to the member variable so that it can be used by 
            //test methods.
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
