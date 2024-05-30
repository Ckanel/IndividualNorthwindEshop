using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData.Models;
using CommonData.Data;
using Microsoft.AspNetCore.Authorization;
using IndividualNorthwindEshop.Services;
using X.PagedList;

namespace IndividualNorthwindEshop.Controllers
{
    [Authorize(Roles = "Manager,Employee")]
    public class CustomersController : Controller
    {
        private readonly MasterContext _context;
        private readonly ILogger<CustomersController> _logger;

        // Constructor
        public CustomersController(MasterContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int pageSize = 10)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CustomerIdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "customerId_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var customers = from c in _context.Customers
                            select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.CompanyName.Contains(searchString)
                    || c.ContactName.Contains(searchString)
                    || c.City.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "customerId_desc":
                    customers = customers.OrderByDescending(c => c.CustomerId);
                    break;
                default:
                    customers = customers.OrderBy(c => c.CustomerId);
                    break;
            }

            return View(await customers.ToPagedListAsync(pageNumber ?? 1, pageSize));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }



        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            _logger.LogInformation("Edit method called with id: {Id}", id);

            if (id == null)
            {
                _logger.LogWarning("Edit method called with null id.");
                return NotFound();
            }

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with id: {Id} not found.", id);
                    return NotFound();
                }
                _logger.LogInformation("Customer with id: {Id} found. Returning edit view.", id);
                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer with id: {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerId,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            _logger.LogInformation("Edit POST method called with id: {Id}", id);

            if (id != customer.CustomerId)
            {
                _logger.LogWarning("Customer ID from URL ({UrlId}) does not match Customer ID from form ({FormId}).", id, customer.CustomerId);
                return NotFound();
            }
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Model state is valid for customer with id: {Id}", id);

                try
                {
                    _logger.LogInformation("Attempting to update customer with id: {Id} in the database.", id);
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated customer with id: {Id} in the database.", id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "DbUpdateConcurrencyException occurred while updating customer with id: {Id}", id);

                    if (!CustomerExists(customer.CustomerId))
                    {
                        _logger.LogWarning("Customer with id: {Id} does not exist.", customer.CustomerId);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency issue: Exception thrown while updating customer with id: {Id}", id);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while updating customer with id: {Id}", id);
                    return StatusCode(500, "Internal server error");
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                _logger.LogWarning("Model state is invalid for customer with id: {Id}.", id);
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error in field {Field}: {Error}", state.Key, error.ErrorMessage);
                    }
                }
            }

            _logger.LogInformation("Returning edit view for customer with id: {Id} due to model state errors or mismatched IDs.", id);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders) // Include related orders
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Fetch related Carts
            var carts = await _context.Carts.Where(c => c.CustomerId == id).ToListAsync();

            // Remove related Carts
            if (carts.Any())
            {
                _context.Carts.RemoveRange(carts);
            }

            // Remove related Orders
            if (customer.Orders.Any())
            {
                _context.Orders.RemoveRange(customer.Orders);
            }

            // Remove Customer
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
