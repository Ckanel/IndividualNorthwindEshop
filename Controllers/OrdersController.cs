using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IndividualNorthwindEshop.Models;
using IndividualNorthwindEshop.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IndividualNorthwindEshop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MasterContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(MasterContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Orders
        [Authorize(Roles="Manager,Employee")]
        public async Task<IActionResult> Index()
        {
            var masterContext = _context.Orders.Include(o => o.Customer).Include(o => o.Employee).Include(o => o.ShipViaNavigation);
            return View(await masterContext.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        //Customer Orders 
        [Authorize(Roles = "Customer")]
        public IActionResult CustomerOrders()
        {
            var customerId = User.FindFirstValue("CustomerId");

            var myOrders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new MyOrdersViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = (DateTime)o.OrderDate,
                    ProductName = o.OrderDetails.First().Product.ProductName,
                    Quantity = o.OrderDetails.First().Quantity,
                    UnitPrice = o.OrderDetails.First().UnitPrice,
                    TotalPrice = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity),
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipPostalCode = o.ShipPostalCode,
                    ShipCountry = o.ShipCountry,
                    GuestEmail = o.GuestEmail                    
                })
                .ToList();

            return View(myOrders);
        }

        
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult PendingOrders()
        {
            var pendingOrders = _context.Orders
                .Where(o => !o.IsHandled && o.Status == "Pending")
                .ToList();
            return View(pendingOrders);
        }
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> HandleOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.IsBeingHandled)
            {
                TempData["ErrorMessage"] = "This order is already being handled by another employee.";
                return RedirectToAction("PendingOrders");
            }

            order.IsBeingHandled = true;
            order.Status = "Being Reviewed";
            order.HandlingStartTime = DateTime.Now;
            await _context.SaveChangesAsync();

            var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
            var currentProducts = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    _context.Entry(product).Property("Timestamp").OriginalValue = product.Timestamp;
                }
            }

            try
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                    if (product != null)
                    {
                        product.ReservedStock += orderDetail.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "A concurrency conflict occurred while updating the reserved stock for order with ID: {OrderId}", id);
                TempData["ErrorMessage"] = "A concurrency conflict occurred while updating the order. Please try again.";
                return RedirectToAction("PendingOrders");
            }

            var viewModel = new HandleOrderViewModel
            {
                Order = order,
                Products = order.OrderDetails.Select(od => new ProductViewModel
                {
                    ProductId = od.Product?.ProductId ?? 0,
                    ProductName = od.Product?.ProductName ?? "Unknown",
                    UnitPrice = od.UnitPrice,
                    CategoryId = od.Product?.CategoryId ?? 0,
                    CategoryName = od.Product?.Category?.CategoryName ?? "Unknown",
                    UnitsInStock = od.Product?.UnitsInStock ?? 0,
                    Discontinued = od.Product?.Discontinued ?? false
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> CloseOrder(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                Order order = null;

                try
                {
                    order = await _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.OrderDetails)
                            .ThenInclude(od => od.Product)
                        .FirstOrDefaultAsync(o => o.OrderId == id);

                    if (order == null)
                    {
                        return NotFound();
                    }
                    if (!order.IsBeingHandled)
                    {
                        order.IsBeingHandled = true;
                        order.Status = "Being Reviewed";
                        order.HandlingStartTime = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }


                    var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                    var currentProducts = await _context.Products
                        .Where(p => productIds.Contains(p.ProductId))
                        .ToListAsync();

                    bool stockSufficient = true;
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                        if (product == null)
                        {
                            stockSufficient = false;
                            break;
                        }

                        int availableStock = (product.UnitsInStock ?? 0) - product.ReservedStock;
                        if (orderDetail.Quantity > availableStock)
                        {
                            stockSufficient = false;
                            break;
                        }
                    }

                    if (stockSufficient)
                    {
                        foreach (var orderDetail in order.OrderDetails)
                        {
                            var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                            if (product != null)
                            {
                                product.UnitsInStock -= orderDetail.Quantity;
                                product.ReservedStock -= orderDetail.Quantity;
                            }
                        }

                        order.Status = "Completed";
                        order.IsHandled = true;
                        order.IsBeingHandled = false;
                        order.HandlingStartTime = null;

                        _context.Entry(order).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        // Send notification and log order completion
                        _ = SendOrderCompletionNotification(order);
                        _logger.LogInformation("Order with ID: {OrderId} has been completed.", order.OrderId);

                        return RedirectToAction("ClosedOrder", new { id = order.OrderId });
                    }
                    else
                    {
                        await transaction.RollbackAsync();

                        TempData["ErrorMessage"] = "Insufficient stock to complete the order.";
                        return RedirectToAction("HandleOrder", new { id = order.OrderId });
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "A concurrency conflict occurred while updating the order with ID: {OrderId}", id);

                    // Refresh the order entity from the database
                    _context.Entry(order).Reload();

                    // Check if the order has already been completed by another employee
                    if (order.IsHandled && order.Status == "Completed")
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "This order has already been completed by another employee.";
                        return RedirectToAction("PendingOrders");
                    }

                    // Retry the update operation
                    order.Status = "Completed";
                    order.IsHandled = true;
                    order.IsBeingHandled = false;
                    order.HandlingStartTime = null;

                    _context.Entry(order).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Send notification and log order completion
                    _ = SendOrderCompletionNotification(order);
                    _logger.LogInformation("Order with ID: {OrderId} has been completed.", order.OrderId);

                    return RedirectToAction("ClosedOrder", new { id = order.OrderId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "An error occurred while closing the order with ID: {OrderId}", id);
                    TempData["ErrorMessage"] = "An error occurred while closing the order. Please try again.";
                    return RedirectToAction("HandleOrder", new { id = order?.OrderId ?? 0 });
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> ClosedOrder(int id)
        {
            // Retrieve the closed order details based on the provided ID
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        












        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> UpdateOrderQuantities(int id, List<OrderDetail> orderDetails)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!order.IsBeingHandled)
            {
                order.IsBeingHandled = true;
                order.Status = "Being Reviewed";
                order.HandlingStartTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
            var currentProducts = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var updatedOrderDetail in orderDetails)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == updatedOrderDetail.ProductId);
                if (orderDetail != null)
                {
                    var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                    if (product != null)
                    {
                        _context.Entry(product).Property("Timestamp").OriginalValue = product.Timestamp;
                    }
                }
            }

            try
            {
                foreach (var updatedOrderDetail in orderDetails)
                {
                    var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == updatedOrderDetail.ProductId);
                    if (orderDetail != null)
                    {
                        var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                        if (product != null)
                        {
                            int quantityChange = updatedOrderDetail.Quantity - orderDetail.Quantity;
                            product.ReservedStock += quantityChange;
                            orderDetail.Quantity = updatedOrderDetail.Quantity;
                        }
                    }
                }
                order.IsBeingHandled = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order quantities updated for order with ID: {OrderId}", order.OrderId);
                TempData["SuccessMessage"] = "Order quantities updated successfully.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "A concurrency conflict occurred while updating order quantities for order with ID: {OrderId}", id);
                TempData["ErrorMessage"] = "A concurrency conflict occurred while updating the order quantities. Please try again.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order quantities for order with ID: {OrderId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating order quantities. Please try again.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> EditOrderQuantities(int id)
        {
            var order = await _context.Orders
    .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Product)
    .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Being Reviewed")
            {
                TempData["ErrorMessage"] = "Order quantities can only be updated for orders in the 'Being Reviewed' status.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }

            return View(order);
        }






        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
                if (!order.IsBeingHandled)
                {
                    order.IsBeingHandled = true;
                    order.Status = "Being Reviewed";
                    order.HandlingStartTime = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                
            

            var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
            var currentProducts = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToListAsync();

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    _context.Entry(product).Property("Timestamp").OriginalValue = product.Timestamp;
                }
            }

            try
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                    if (product != null)
                    {
                        product.ReservedStock -= orderDetail.Quantity;
                    }
                }

               
                await _context.SaveChangesAsync();

                _ = SendOrderCancellationNotification(order);
                _logger.LogInformation("Order with ID: {OrderId} has been cancelled.", order.OrderId);

                return RedirectToAction("OrderCancelled", new { id = order.OrderId });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "A concurrency conflict occurred while cancelling the order with ID: {OrderId}", id);
                TempData["ErrorMessage"] = "A concurrency conflict occurred while cancelling the order. Please try again.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cancelling the order with ID: {OrderId}", id);
                TempData["ErrorMessage"] = "An error occurred while cancelling the order. Please try again.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> ConfirmCancelOrder(int id, string cancellationReason)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }
            order.Status = "Cancelled";
            order.IsHandled = true;
            order.IsBeingHandled = false;
            order.HandlingStartTime = null;
            order.CancellationReason = cancellationReason;

            // Revert the stock and reserved stock for each product in the order
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product != null)
                {
                    product.UnitsInStock += orderDetail.Quantity;
                    product.ReservedStock -= orderDetail.Quantity;
                }
            }

            await _context.SaveChangesAsync();


            return RedirectToAction("PendingOrders", "Orders");
        }
        [HttpGet]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> OrderCancelled(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }



        public void ResetStaleOrders()
        {
            var staleOrders = _context.Orders
                .Where(o => o.Status == "BeingReviewed" && o.IsBeingHandled &&
                            o.HandlingStartTime < DateTime.Now.AddMinutes(-10))
                .ToList();

            foreach (var order in staleOrders)
            {
                order.IsBeingHandled = false;
                order.Status = "Pending";
                order.HandlingStartTime = null;
            }

            _context.SaveChanges();
        }
        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> RevertToPending(int id)
        {
            var order = await _context.Orders
        .Include(o => o.OrderDetails)
        .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _context.Products.FindAsync(orderDetail.ProductId);
                if (product != null)
                {
                    product.UnitsInStock += orderDetail.Quantity;
                    product.ReservedStock -= orderDetail.Quantity;
                }
            }

            order.Status = "Pending";
            order.IsHandled = false;
            order.IsBeingHandled = false;
            order.HandlingStartTime = null;
           
            await _context.SaveChangesAsync();

            // Redirect to the pending orders page or any other appropriate page
            return RedirectToAction("PendingOrders", "Orders");
        }






        private async Task SendOrderCompletionNotification(Order order)
        {
            try
            {
                // Implement your logic to send the order completion notification
                // For example, you can send an email or push notification to the customer
                // You can use a notification service, email service, or any other means of communication

                // Example code for sending an email notification:
                // var emailService = new EmailService();
                // var emailContent = $"Dear customer,\n\nYour order with ID: {order.OrderId} has been completed.\n\nThank you for your purchase!";
                // await emailService.SendEmailAsync(order.CustomerEmail, "Order Completed", emailContent);

                _logger.LogInformation("Order completion notification sent for order with ID: {OrderId}", order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order completion notification for order with ID: {OrderId}", order.OrderId);
            }
        }

        private async Task SendOrderCancellationNotification(Order order)
        {
            try
            {
                // Implement your logic to send the order cancellation notification
                // For example, you can send an email or push notification to the customer
                // You can use a notification service, email service, or any other means of communication

                // Example code for sending an email notification:
                // var emailService = new EmailService();
                // var emailContent = $"Dear customer,\n\nYour order with ID: {order.OrderId} has been completed.\n\nThank you for your purchase!";
                // await emailService.SendEmailAsync(order.CustomerEmail, "Order Completed", emailContent);

                _logger.LogInformation("Order completion notification sent for order with ID: {OrderId}", order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order cancellation notification for order with ID: {OrderId}", order.OrderId);
            }
        }


    }
}
