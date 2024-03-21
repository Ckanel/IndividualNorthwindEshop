﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IndividualNorthwindEshop.Data;
using IndividualNorthwindEshop.Models;
using IndividualNorthwindEshop.Services;
using Microsoft.AspNetCore.Authorization;

namespace IndividualNorthwindEshop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MasterContext _context;
        private readonly IOle78DecryptionService _decryptionService;
        public CategoriesController(MasterContext context, IOle78DecryptionService decryptionService)
        {
            _context = context;
            _decryptionService = decryptionService;
        }

        // GET: Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [AllowAnonymous]
        public IActionResult GetEmployeePhoto(int id)
        {
            var employee = _context.Categories.FirstOrDefault(e => e.CategoryId == id);
            if (employee != null && employee.Picture != null)
            {
                if (id >= 1 && id <= 9)
                {
                    byte[] decryptedPhoto = _decryptionService.DecryptData(employee.Picture);
                    if (decryptedPhoto != null)
                    {
                        return File(decryptedPhoto, "image/bmp");
                    }
                }
                else
                {
                    return File(employee.Picture, "image/bmp");
                }
            }
            return NotFound();
        }

        // GET: Categories/Create
        [Authorize(Roles = "Employee,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see 
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,Description")] Category category, IFormFile picture)
        {
            if (ModelState.IsValid)
            {
                if (picture != null && picture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await picture.CopyToAsync(memoryStream);
                        category.Picture = memoryStream.ToArray();
                    }
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        // GET: Categories/Edit/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description,Picture")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
