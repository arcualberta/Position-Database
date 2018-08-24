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
        public ReportService(ApplicationDbContext db)
            :base(db)
        {
            
        }

        public List<FacultyEmployeeViewModel> GetFacultyMembers(PersonFilter filter, int? page = null, int? offset = null)
        {
            IQueryable<PersonPosition> associations = GetPersonPositionAssociations(filter.PositionType, filter.Date, filter.IsActive);
            associations.OrderBy(a => a.Person.Name);

            throw new NotImplementedException();
        }
    }
}
