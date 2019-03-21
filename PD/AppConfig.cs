using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD
{
    public class AppConfig
    {
        public readonly string[] FacultyTypes;

        public AppConfig(IConfiguration configuration)
        {
            FacultyTypes = configuration.GetSection("PositionTypes:Faculty").Get<string[]>();
        }




    }
}
