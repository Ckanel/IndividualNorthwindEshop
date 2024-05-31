using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData.Data;
using CommonData.Models;
using IndividualNorthwindEshop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace IndividualNorthwindEshop.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MasterContext _context;
        private readonly IOle78DecryptionService _decryptionService;
        private readonly ILogger<CategoriesController> _logger;
        public CategoriesController(MasterContext context, IOle78DecryptionService decryptionService, ILogger<CategoriesController> logger)
        {
            _context = context;
            _decryptionService = decryptionService;
            _logger = logger;
        }

        // GET: Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "Employee,Manager")]
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
               
                
                    return File(employee.Picture, "image/bmp");
                
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

                try
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Category created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx &&
                        sqlEx.Message.Contains("String or binary data would be truncated"))
                    {
                        TempData["ErrorMessage"] = "The category name is too long. Please shorten it to 15 characters or less.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the category. Please try again.";
                    }
                    // Log the exception
                    _logger.LogError(ex, "An error occurred while creating a category.");

                    
                   
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Description")] Category category, IFormFile picture)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning($"Edit attempt with mismatched id. Id: {id}, CategoryId: {category.CategoryId}");
                return NotFound();
            }

            // Log the initial ModelState
            LogModelState();

            // Remove the picture key from ModelState if no new picture is uploaded
            if (picture == null || picture.Length == 0)
            {
                _logger.LogInformation("No new picture uploaded, keeping the existing picture.");
                ModelState.Remove(nameof(category.Picture));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (picture != null && picture.Length > 0)
                    {
                        _logger.LogInformation($"Processing picture upload for category {category.CategoryId}");
                        using (var memoryStream = new MemoryStream())
                        {
                            await picture.CopyToAsync(memoryStream);
                            category.Picture = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Preserve the existing picture if no new file is uploaded
                        var existingCategory = await _context.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.CategoryId == category.CategoryId);

                        if (existingCategory != null)
                        {
                            category.Picture = existingCategory.Picture;
                        }
                    }

                    _logger.LogInformation($"Updating category with id {category.CategoryId}");
                    _context.Update(category);
                    TempData["SuccessMessage"] = "Category updated successfully.";
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully updated category {category.CategoryId}");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, $"Concurrency error updating category with id {category.CategoryId}");

                    if (!CategoryExists(category.CategoryId))
                    {
                        _logger.LogWarning($"Category with id {category.CategoryId} does not exist.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("String or binary data would be truncated"))
                    {
                        TempData["ErrorMessage"] = "The category name is too long. Please shorten it to 15 characters or less.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while saving the category. Please try again.";
                    }
                    _logger.LogError(ex, $"Error updating category with id {category.CategoryId}");
                }
            }

            _logger.LogWarning("Model state is not valid.");
            LogModelState(); // Log the ModelState errors again after attempting to update the category
            return View(category);
        }

        private void LogModelState()
        {
            foreach (var state in ModelState)
            {
                var key = state.Key;
                var errors = state.Value.Errors;
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        _logger.LogError("ModelState Error - Key: {Key}, Error: {ErrorMessage}", key, error.ErrorMessage);
                        if (error.Exception != null)
                        {
                            _logger.LogError(error.Exception, "Exception for ModelState error with Key: {Key}", key);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("ModelState Key: {Key} is valid.", key);
                }
            }
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
        // Categories-Products controller action
        //[AllowAnonymous]
        //public IActionResult ProductsByCategories(int categoryId)
        //{
        //    var products = _context.Products
        //        .Where(p => p.CategoryId == categoryId)
        //        .Select(p => new ProductViewModel
        //        {
        //            ProductId = p.ProductId,
        //            ProductName = p.ProductName,
        //            UnitPrice = p.UnitPrice,
        //            CategoryId = p.CategoryId,
        //            CategoryName = p.Category.CategoryName
        //        })
        //        .ToList();

        //    return View(products);
        //}
        [AllowAnonymous]
        public IActionResult ProductsByCategories(int categoryId)
        {
            var products = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    UnitsInStock = p.UnitsInStock,
                    Discontinued = p.Discontinued
                })
                .ToList();

            return View(products);
        }
    }
}
