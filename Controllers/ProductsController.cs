using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData.Data;
using CommonData.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace IndividualNorthwindEshop.Controllers
{
    
    public class ProductsController : Controller
    {
        private readonly MasterContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(MasterContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, decimal? minPrice, decimal? maxPrice, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var products = from p in _context.Products.Include(p => p.Category)
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.UnitPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.UnitPrice <= maxPrice.Value);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "Price":
                    products = products.OrderBy(p => p.UnitPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.UnitPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            int pageSize = 25;
            return View(await products.ToPagedListAsync(pageNumber ?? 1, pageSize));
        }
    
    public IActionResult GetProductPhoto(int id)
        {
            var product = _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new { p.Photo, p.ProductName })
                .FirstOrDefault();

            if (product == null || product.Photo == null)
            {
                return NotFound();
            }

            return File(product.Photo, "image/jpeg");
        }
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [Authorize(Roles = "Employee,Manager")]
        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product, IFormFile photo)
        {
            ModelState.Remove("Timestamp");
            if (ModelState.IsValid)
            {
                try
                {

                    
                    if (photo != null && photo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await photo.CopyToAsync(memoryStream);
                        product.Photo = memoryStream.ToArray();
                    }

                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product {ProductId} created successfully.", product.ProductId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating the product {ProductId}.", product.ProductId);
                    ModelState.AddModelError("", "Unable to create product. Please try again later.");
                }
            }

            LogModelStateErrors();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued,Timestamp")] Product product, IFormFile photo)
        {
            if (id != product.ProductId)
            {
                _logger.LogWarning($"Edit attempt with mismatched id. Id: {id}, ProductId: {product.ProductId}");
                return NotFound();
            }

            // Log the initial ModelState
            LogModelState();

            if (photo == null || photo.Length == 0)
            {
                _logger.LogInformation("No new photo uploaded, keeping the existing photo.");
                ModelState.Remove(nameof(product.Photo)); // Removes "Photo" from ModelState validation
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        _logger.LogInformation($"Processing photo upload for product {product.ProductId}");
                        using (var stream = new MemoryStream())
                        {
                            await photo.CopyToAsync(stream);
                            product.Photo = stream.ToArray();
                        }
                    }
                    else
                    {
                        // Preserve the existing photo if no new file is uploaded
                        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                        if (existingProduct != null)
                        {
                            product.Photo = existingProduct.Photo;
                        }
                    }

                    _logger.LogInformation($"Updating product with id {product.ProductId}");
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully updated product {product.ProductId}");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        _logger.LogWarning($"Product with id {product.ProductId} does not exist.");
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating product with id {product.ProductId}");
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit was modified by another user after you got the original value. The edit operation was aborted.");
                        return View(product);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model state is not valid.");
            LogModelState(); // Log the ModelState errors again after attempting to update the product

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", product.SupplierId);
            return View(product);
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

    



        private void LogModelStateErrors()
        {
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        _logger.LogWarning("ModelState error for {Key}: {ErrorMessage}", key, error.ErrorMessage);
                    }
                }
            }
        }
        // GET: Products/Delete/5
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Product {ProductId} deleted successfully.", id);
                }
                else
                {
                    _logger.LogWarning("Product with id {Id} not found for deletion.", id);
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product {ProductId}.", id);
                ModelState.AddModelError("", "Unable to delete product. Please try again later.");
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
        


    }
}
