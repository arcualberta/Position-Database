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

        public readonly List<SalaryScale> _salaryScales;

        public SalaryScaleService(ApplicationDbContext ctx)
        {
            _context = ctx;
            _salaryScales = _context.SalaryScales.ToList();
        }

        public SalaryScale GetSalaryScale(string positionTitle, DateTime targetDate)
        {
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
