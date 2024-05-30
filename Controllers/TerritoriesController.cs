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

namespace IndividualNorthwindEshop.Controllers
{
    [Authorize(Roles = "Manager,Employee")]
    public class TerritoriesController : Controller
    {
        private readonly MasterContext _context;
        private readonly ILogger<TerritoriesController> _logger;
        public TerritoriesController(MasterContext context, ILogger<TerritoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Territories
        public async Task<IActionResult> Index()
        {
            var masterContext = _context.Territories.Include(t => t.Region);
            return View(await masterContext.ToListAsync());
        }

        // GET: Territories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var territory = await _context.Territories
                .Include(t => t.Region)
                .FirstOrDefaultAsync(m => m.TerritoryId == id);
            if (territory == null)
            {
                return NotFound();
            }

            return View(territory);
        }

        // GET: Territories/Create
        public IActionResult Create()
        {
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionDescription");
            return View();
        }

        // POST: Territories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerritoryId,TerritoryDescription,RegionId")] Territory territory)
        {
            try
            {
                _logger.LogInformation("Attempting to create a new territory with ID: {TerritoryId}, Description: {TerritoryDescription}, RegionId: {RegionId}",
                    territory.TerritoryId, territory.TerritoryDescription, territory.RegionId);

                territory.Region = await _context.Regions.FindAsync(territory.RegionId);

                if (territory.Region == null)
                {
                    ModelState.AddModelError("Region", "The selected Region is invalid.");
                    _logger.LogWarning("The selected Region is invalid.");
                }
                else
                {
                    _logger.LogInformation("Region is valid.");
                }

                // Remove the Region property from ModelState validation
                ModelState.Remove(nameof(territory.Region));

                if (ModelState.IsValid)
                {
                    _context.Add(territory);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully created territory with ID: {TerritoryId}", territory.TerritoryId);
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogWarning("Failed to create territory, ModelState is invalid.");
                LogModelErrors();

                ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionDescription", territory.RegionId);
                return View(territory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new territory with ID: {TerritoryId}", territory.TerritoryId);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the territory. Please try again.");
                ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionDescription", territory.RegionId);
                return View(territory);
            }
        }

        private void LogModelErrors()
        {
            foreach (var entry in ModelState)
            {
                var key = entry.Key;
                var errors = entry.Value.Errors;
                foreach (var error in errors)
                {
                    _logger.LogError("ModelState Error - Key: {Key}, Error: {ErrorMessage}", key, error.ErrorMessage);
                    if (error.Exception != null)
                    {
                        _logger.LogError(error.Exception, "Exception for ModelState error");
                    }
                }
            }
        }




        // GET: Territories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var territory = await _context.Territories.FindAsync(id);
            if (territory == null)
            {
                return NotFound();
            }
            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionDescription", territory.RegionId);
            return View(territory);
        }

        // POST: Territories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TerritoryId,TerritoryDescription,RegionId")] Territory territory)
        {
            _logger.LogInformation("Edit action called with ID: {Id}", id);

            if (id != territory.TerritoryId)
            {
                _logger.LogWarning("ID mismatch: ID in URL ({Id}) does not match ID in model ({TerritoryId})", id, territory.TerritoryId);
                return NotFound();
            }

            territory.Region = await _context.Regions.FindAsync(territory.RegionId);

            if (territory.Region == null)
            {
                ModelState.AddModelError("Region", "The selected Region is invalid.");
                _logger.LogWarning("The selected Region is invalid.");
            }
            else
            {
                _logger.LogInformation("Region is valid.");
            }

            // Remove the Region property from ModelState validation
            ModelState.Remove(nameof(territory.Region));

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Updating territory: {Territory}", territory);
                    _context.Update(territory);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Territory with ID: {TerritoryId} updated successfully", territory.TerritoryId);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency exception occurred while updating territory with ID: {TerritoryId}", territory.TerritoryId);

                    if (!TerritoryExists(territory.TerritoryId))
                    {
                        _logger.LogWarning("Territory with ID: {TerritoryId} no longer exists", territory.TerritoryId);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model state is invalid. Logging ModelState errors:");
            LogModelErrors();

            ViewData["RegionId"] = new SelectList(_context.Regions, "RegionId", "RegionDescription", territory.RegionId);
            return View(territory);
        }







        // GET: Territories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var territory = await _context.Territories
                .Include(t => t.Region)
                .FirstOrDefaultAsync(m => m.TerritoryId == id);

            if (territory == null)
            {
                return NotFound();
            }

            return View(territory);
        }

        // POST: Territories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var territory = await _context.Territories.FindAsync(id);
            _context.Territories.Remove(territory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerritoryExists(string id)
        {
            return _context.Territories.Any(e => e.TerritoryId == id);
        }
    }
}
