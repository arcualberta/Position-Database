using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using PD.Data;
using PD.Models;
using PD.Models.AppViewModels.Faculty;
using PD.Models.AppViewModels.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public class ReportService : PdServiceBase
    {
        public ReportService(ApplicationDbContext db, IPdDataProtector dataProtector)
            : base(db, dataProtector)
        {
        }
    }
}
