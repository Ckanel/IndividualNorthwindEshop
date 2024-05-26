using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData.Models;
using CommonData.Data;
using System.Diagnostics;
using IndividualNorthwindEshop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IndividualNorthwindEshop.Controllers
{
    [Authorize(Roles = "Manager")]
    public class EmployeesController : Controller
    {
        private readonly MasterContext _context;
        private readonly IOle78DecryptionService _decryptionService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(MasterContext context, IOle78DecryptionService decryptionService, ILogger<EmployeesController> logger)
        {
            _context = context;
            _decryptionService = decryptionService;
            _logger = logger;
        }
        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var masterContext = _context.Employees.Include(e => e.ReportsToNavigation);
            return View(await masterContext.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        public IActionResult GetEmployeePhoto(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee != null && employee.Photo != null)
            {
                if (id >= 1 && id <= 9)
                {
                    byte[] decryptedPhoto = _decryptionService.DecryptData(employee.Photo);
                    if (decryptedPhoto != null)
                    {
                        return File(decryptedPhoto, "image/bmp");
                    }
                }else
                {
                    return File(employee.Photo, "image/bmp");
                }

            }
            return NotFound();
        }

       

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ReportsTo);
            return View(employee);
        }


        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,LastName,FirstName,Title,TitleOfCourtesy,BirthDate,HireDate,Address,City,Region,PostalCode,Country,HomePhone,Extension,Notes,ReportsTo,PhotoPath")] Employee employee, IFormFile photo)
        {
            if (id != employee.EmployeeId)
            {
                _logger.LogWarning("Edit operation failed: URL id {UrlId} does not match employee id {EmployeeId}.", id, employee.EmployeeId);
                return NotFound();
            }

            // Remove the 'User' property from ModelState as it is not part of the form binding
            ModelState.Remove("User");

            // Remove the 'Photo' from ModelState to handle optional uploads
            ModelState.Remove("Photo");

            if (ModelState.IsValid)
            {
                try
                {
                    if (photo != null && photo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await photo.CopyToAsync(memoryStream);
                            employee.Photo = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Retrieve existing photo if available
                        var existingEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeId == id);
                        if (existingEmployee != null)
                        {
                            employee.Photo = existingEmployee.Photo;
                            employee.PhotoPath = existingEmployee.PhotoPath;
                        }
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Employee with id {EmployeeId} has been updated successfully.", employee.EmployeeId);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmployeeExists(employee.EmployeeId))
                    {
                        _logger.LogWarning("Edit operation failed: Employee with id {EmployeeId} does not exist.", employee.EmployeeId);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Edit operation failed due to a concurrency exception for employee id {EmployeeId}.", employee.EmployeeId);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while updating employee id {EmployeeId}.", employee.EmployeeId);
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Log each ModelState error
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    _logger.LogWarning("ModelState error for {Key}: {ErrorMessage}", key, error.ErrorMessage);
                    Debug.WriteLine($"ModelState error for {key}: {error.ErrorMessage}");
                }
            }

            _logger.LogWarning("Edit operation failed due to invalid model state for employee id {EmployeeId}.", employee.EmployeeId);
            ViewData["ReportsTo"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ReportsTo);
            return View(employee);
        }



        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ReportsToNavigation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogInformation("Employee with id {EmployeeId} not found.", id);
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == id); // Assuming _context.Users.DbSet<AppUser> exists
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User associated with employee Id {EmployeeId} has been deleted.", id);
                }

                // Proceed to delete the Employee record from the context (database)
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Employee with id {EmployeeId} has been deleted successfully.", id);
            }
            catch (Exception ex)
            {
                // Log the exception with _logger
                _logger.LogError(ex, "An error occurred while deleting the employee with id {EmployeeId}.", id);

                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the employee. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            // If we reach this point, the deletion was successful
            // Redirect to the Index page
            return RedirectToAction(nameof(Index));
        }




        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
