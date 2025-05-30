using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetCareMVC.Data;
using PetCareMVC.Models;
using Microsoft.AspNetCore.Authorization;
using PetCareMVC.Helper;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace PetCareMVC.Controllers
{
    [Authorize]
    public class PetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PetsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchName, int? pageNumber)
        {
            // Текущите стойности отиват в ViewData и после ги ползваме в изгледа
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilterName"] = searchName;
            ViewData["CurrentSort"] = sortOrder;

            var pets = from p in _context.Pets.Include(p => p.Owner)
                       select p;

            if (!string.IsNullOrEmpty(searchName))
                pets = pets.Where(p => p.Name.Contains(searchName));

            pets = sortOrder == "name_desc"
                ? pets.OrderByDescending(p => p.Name)
                : pets.OrderBy(p => p.Name);

            int pageSize = 5;

            return View(await PaginatedList<Pet>.CreateAsync(pets.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .Include(p => p.HealthRecords)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Address");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,Breed,DateOfBirth,WeightKg,OwnerId")] Pet pet, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Път до wwwroot/images
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;
                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    pet.PhotoPath = "/images/" + fileName;
                }

                _context.Add(pet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OwnerId"] = new SelectList(_context.Owners.Select(o => new{o.Id,FullName = o.FirstName + " " + o.LastName}), "Id", "FullName");

            return View(pet);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Address", pet.OwnerId);
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,Breed,DateOfBirth,WeightKg,OwnerId")] Pet pet, IFormFile? ImageFile)
        {
            if (id != pet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string extension = Path.GetExtension(ImageFile.FileName);
                        fileName = fileName + "_" + Guid.NewGuid().ToString() + extension;
                        string path = Path.Combine(wwwRootPath, "images", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        pet.PhotoPath = "/images/" + fileName;
                    }

                    _context.Update(pet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PetExists(pet.Id))
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

            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", pet.OwnerId);
            return View(pet);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pet = await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pet == null)
            {
                return NotFound();
            }

            return View(pet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.Id == id);
        }
    }
}
