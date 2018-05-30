using PAD.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAD.Services
{
    /// <summary>
    /// PadServiceBase: base class for all PAD service classes
    /// </summary>
    public class PadServiceBase
    {
        public ApplicationDbContext Db { get; }

        public PadServiceBase(ApplicationDbContext db)
        {
            Db = db;
        }
    }
}
