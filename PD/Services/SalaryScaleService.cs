using PD.Data;
using PD.Models.SalaryScales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PD.Services
{
    public class SalaryScaleService
    {
        public ApplicationDbContext _context { get; }

        public SalaryScaleService(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        private List<SalaryScale> _salaryScales;
        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
            if (_salaryScales == null)
                _salaryScales = _context.SalaryScales.ToList();

            var scale = _salaryScales.Where(sc => 
                    sc.StartDate <= targetDate 
                    && sc.EndDate >= targetDate 
                    && sc.Category == positionTitle)
                .FirstOrDefault();

            if (scale == null)
                throw new Exception($"No {positionTitle} salary scale not found for the year containing {targetDate.ToString("yyyy-MM-dd")}");

            return scale;
        }
    }
}
