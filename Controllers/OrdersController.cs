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
        //Pending orders 
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult PendingOrders()
        {
            var pendingOrders = _context.Orders
                .Where(o => !o.IsHandled)
                .ToList();

            return View(pendingOrders);
        }
        // OrdersController.cs
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult HandleOrder(int id)
        {
            var order = _context.Orders.Include(o => o.OrderDetails)
                                       .ThenInclude(od => od.Product)
                                       .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = orderDetail.Product;
                product.ReservedStock += orderDetail.Quantity;
            }

            _context.SaveChanges();
            var viewModel = new HandleOrderViewModel
            {
                Order = order,
                Products = order.OrderDetails.Select(od => new ProductViewModel
                {
                    ProductId = od.Product.ProductId,
                    ProductName = od.Product.ProductName,
                    UnitPrice = od.UnitPrice,
                    CategoryId = od.Product.CategoryId,
                    CategoryName = od.Product.Category.CategoryName,
                    UnitsInStock = od.Product.UnitsInStock,
                    Discontinued = od.Product.Discontinued
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public IActionResult UpdateOrderQuantities(int id, List<OrderDetail> orderDetails)
        {
            var order = _context.Orders.Include(o => o.OrderDetails)
                                       .ThenInclude(od => od.Product)
                                       .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            foreach (var updatedOrderDetail in orderDetails)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == updatedOrderDetail.ProductId);
                if (orderDetail != null)
                {
                    var product = orderDetail.Product;
                    int quantityChange = updatedOrderDetail.Quantity - orderDetail.Quantity;
                    product.ReservedStock += quantityChange;
                    orderDetail.Quantity = updatedOrderDetail.Quantity;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("HandleOrder", new { id = order.OrderId });
        }




       
        [Authorize(Roles = "Manager,Employee")]
        [HttpPost]
        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> CloseOrder(int id)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.Orders.Include(o => o.OrderDetails)
                                               .ThenInclude(od => od.Product)
                                               .FirstOrDefault(o => o.OrderId == id);

                    if (order == null)
                    {
                        return NotFound();
                    }

                    bool stockSufficient = true;
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var product = await _context.Products.FindAsync(orderDetail.ProductId);
                        if (product == null)
                        {
                            stockSufficient = false;
                            break;
                        }

                        _context.Entry(product).State = EntityState.Unchanged;
                        _context.Entry(product).Property(p => p.UnitsInStock).IsModified = true;
                        _context.Entry(product).Property(p => p.ReservedStock).IsModified = true;
                        _context.Entry(product).Property(p => p.Timestamp).OriginalValue = product.Timestamp;

                        int availableStock = (product.UnitsInStock ?? 0) - (product.ReservedStock);
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
                            var product = await _context.Products.FindAsync(orderDetail.ProductId);
                            product.UnitsInStock -= orderDetail.Quantity;
                            product.ReservedStock -= orderDetail.Quantity;
                        }

                        order.Status = "Completed";
                        order.IsHandled = true;
                        await _context.SaveChangesAsync();

                        transaction.Commit();

                        // Send notification and log order completion
                        _ = SendOrderCompletionNotification(order);
                        _logger.LogInformation("Order with ID: {OrderId} has been completed.", order.OrderId);



                        return RedirectToAction("OrderCompleted", new { id = order.OrderId });
                    }
                    else
                    {
                        transaction.Rollback();

                        TempData["ErrorMessage"] = "Insufficient stock to complete the order.";
                        return RedirectToAction("HandleOrder", new { id = order.OrderId });
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    // Display an error message
                    TempData["ErrorMessage"] = "A concurrency conflict occurred while updating the order. Please try again.";
                    _logger.LogError(ex, "A concurrency conflict occurred while updating the order with ID: {OrderId}", id);
                    return RedirectToAction("HandleOrder", new { id = id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    //log the error in case of an unexpected exception
                    _logger.LogError(ex, "An error occurred while closing the order with ID: {OrderId}", id);
                    throw;
                }
            }
        }







        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = _context.Orders.Include(o => o.OrderDetails)
                                       .ThenInclude(od => od.Product)
                                       .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == "Completed")
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = orderDetail.Product;
                    product.UnitsInStock += orderDetail.Quantity;
                }
            }
            else
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = orderDetail.Product;
                    product.ReservedStock -= orderDetail.Quantity;
                }
            }

            order.Status = "Cancelled";
            order.IsHandled = true;
            _context.SaveChanges();

            _ = SendOrderCancellationNotification(order);
            _logger.LogInformation("Order with ID: {OrderId} has been cancelled.", order.OrderId);

            return RedirectToAction("OrderCancelled", new { id = order.OrderId });
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
