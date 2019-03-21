using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Models.SalaryScales;

namespace PD.Controllers
{
    public class SalaryScalesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppConfig _appConfig;

        public SalaryScalesController(ApplicationDbContext context, AppConfig appConfig)
        {
            _context = context;
            _appConfig = appConfig;
        }

        // GET: SalaryScales
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalaryScales.ToListAsync());
        }

        // GET: SalaryScales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryScale = await _context.SalaryScales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryScale == null)
            {
                return NotFound();
            }

            return View(salaryScale);
        }

        // GET: SalaryScales/Create
        public IActionResult Create()
        {
            ViewBag.PositionTypes = _appConfig.FacultyTypes;
            return View();
        }

        // POST: SalaryScales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Minimum,Maximum,StepValue,ContractSettlement,DefaultMeritDecision,IsProjection,StartDate,EndDate")] SalaryScale salaryScale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaryScale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salaryScale);
        }

        // GET: SalaryScales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryScale = await _context.SalaryScales.FindAsync(id);
            if (salaryScale == null)
            {
                return NotFound();
            }
            return View(salaryScale);
        }

        // POST: SalaryScales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Minimum,Maximum,StepValue,ContractSettlement,DefaultMeritDecision,IsProjection,StartDate,EndDate")] SalaryScale salaryScale)
        {
            if (id != salaryScale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaryScale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaryScaleExists(salaryScale.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salaryScale);
        }

        // GET: SalaryScales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaryScale = await _context.SalaryScales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salaryScale == null)
            {
                return NotFound();
            }

            return View(salaryScale);
        }

        // POST: SalaryScales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaryScale = await _context.SalaryScales.FindAsync(id);
            _context.SalaryScales.Remove(salaryScale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryScaleExists(int id)
        {
            return _context.SalaryScales.Any(e => e.Id == id);
        }
    }
}
