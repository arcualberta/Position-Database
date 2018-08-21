﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PD.Data;
using PD.Models;

namespace PD.Controllers
{
    public class ChartStringsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartStringsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ChartStrings
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChartStrings.ToListAsync());
        }

        // GET: ChartStrings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartString = await _context.ChartStrings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chartString == null)
            {
                return NotFound();
            }

            return View(chartString);
        }

        // GET: ChartStrings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChartStrings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,SpeedCode,ComboCode")] ChartString chartString)
        public async Task<IActionResult> Create(ChartString chartString)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chartString);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chartString);
        }

        // GET: ChartStrings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartString = await _context.ChartStrings.SingleOrDefaultAsync(m => m.Id == id);
            if (chartString == null)
            {
                return NotFound();
            }
            return View(chartString);
        }

        // POST: ChartStrings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpeedCode,ComboCode")] ChartString chartString)
        {
            if (id != chartString.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartString);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartStringExists(chartString.Id))
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
            return View(chartString);
        }

        // GET: ChartStrings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartString = await _context.ChartStrings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chartString == null)
            {
                return NotFound();
            }

            return View(chartString);
        }

        // POST: ChartStrings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chartString = await _context.ChartStrings.SingleOrDefaultAsync(m => m.Id == id);
            _context.ChartStrings.Remove(chartString);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartStringExists(int id)
        {
            return _context.ChartStrings.Any(e => e.Id == id);
        }
    }
}