using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCareMVC.Data;
using PetCareMVC.Models;

namespace PetCareMVC.Controllers
{
    [Authorize]
    public class HealthRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HealthRecords.Include(h => h.Pet);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Pet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (healthRecord == null)
            {
                return NotFound();
            }

            return View(healthRecord);
        }

        public IActionResult Create(int? petId)
        {
            if (petId != null)
            {
                ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Name", petId);
            }
            else
            {
                ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Name");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VisitDate,Description,Temperature,IsVaccinated,MicrochipNumber,PetId")] HealthRecord healthRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(healthRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Breed", healthRecord.PetId);
            return View(healthRecord);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord == null)
            {
                return NotFound();
            }
            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Breed", healthRecord.PetId);
            return View(healthRecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VisitDate,Description,Temperature,IsVaccinated,MicrochipNumber,PetId")] HealthRecord healthRecord)
        {
            if (id != healthRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthRecordExists(healthRecord.Id))
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
            ViewData["PetId"] = new SelectList(_context.Pets, "Id", "Breed", healthRecord.PetId);
            return View(healthRecord);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Pet)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (healthRecord == null)
            {
                return NotFound();
            }

            return View(healthRecord);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord != null)
            {
                _context.HealthRecords.Remove(healthRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthRecordExists(int id)
        {
            return _context.HealthRecords.Any(e => e.Id == id);
        }
    }
}
