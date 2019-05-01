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

        public readonly int AuditLogLevel;

        public readonly string AppRoot;

        public AppConfig(IConfiguration configuration)
        {
            FacultyTypes = configuration.GetSection("PositionTypes:Faculty").Get<string[]>();
            AuditLogLevel = string.IsNullOrEmpty(configuration.GetSection("Audit:Level").Value) 
                ? 1 
                : configuration.GetSection("Audit:Level").Get<int>();

            AppRoot = string.IsNullOrEmpty(configuration.GetSection("AppRoot").Value)
                ? ""
                : configuration.GetSection("AppRoot").Value;
        }




    }
}
