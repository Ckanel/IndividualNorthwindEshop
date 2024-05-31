using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CommonData.Models;
using CommonData.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static CommonData.Models.Order;
using IndividualNorthwindEshop.Services;

//using ETL.Extract;
namespace IndividualNorthwindEshop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MasterContext _context;
        private readonly ILogger<OrdersController> _logger;
        private readonly IPaginationService _paginationService;
        private readonly EmailService _emailService;
        //private readonly OrderRepository _orderRepository;
        public OrdersController(MasterContext context, ILogger<OrdersController> logger, IPaginationService paginationService, EmailService emailService)
        {
            _paginationService = paginationService;
            _context = context;
            _logger = logger;
            _emailService = emailService;
            //_orderRepository = _orderRepository;
        }


        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 25)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .AsNoTracking();

            var paginatedList = await _paginationService.CreatePaginatedListAsync(query, pageNumber, pageSize);

            return View(paginatedList);
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
          .Where(o => (int)o.Status == (int)OrderStatus.Pending)
          .Include(o => o.Customer) 
          .ToList();
            return View(pendingOrders);
        }



    
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> HandleOrder(int id)
        {
            bool comingFromUpdate = TempData.ContainsKey("UpdateSuccess") && (bool)TempData["UpdateSuccess"];

            // Clear the TempData right away after using it
            TempData.Remove("UpdateSuccess");
            TempData.Remove("OperationCancelled");

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

            var currentUser = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            _logger.LogInformation("EmployeeId: {EmployeeId}", currentUser?.Value);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Unable to retrieve the current employee's ID.";
                return RedirectToAction("PendingOrders");
            }

            var currentEmployeeId = _context.Users
                .Where(u => u.Id == currentUser.Value)
                .Select(u => u.EmployeeId)
                .FirstOrDefault();

            if (order.Status == OrderStatus.BeingHandled && order.EmployeeId != currentEmployeeId)
            {
                TempData["ErrorMessage"] = "This order is already being handled by another employee.";
                return RedirectToAction("PendingOrders");
            }

            if (order.Status != OrderStatus.BeingHandled && !comingFromUpdate)
            {
                order.EmployeeId = currentEmployeeId;
                order.Status = OrderStatus.BeingHandled;
                order.HandlingStartTime = DateTime.Now;

                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                var currentProducts = await _context.Products.Where(p => productIds.Contains(p.ProductId)).ToListAsync();

                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                    if (product != null)
                    {
                        // Set ReservedStock to the quantity
                        // since this is the first time we are handling the order.

                        product.ReservedStock = orderDetail.Quantity;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency conflict occurred on handling order with ID: {OrderId}", id);
                    TempData["ErrorMessage"] = "A concurrency error occurred. Please try again.";
                    return RedirectToAction("PendingOrders");
                }
            }

            var shippers = await _context.Shippers
        .Select(s => new CustomSelectListItem
        {
            Value = s.ShipperId.ToString(),
            Text = s.CompanyName
        })
        .ToListAsync();

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
                }).ToList(),
                Shippers = shippers
            };

            return View(viewModel);
        }



        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> CloseOrder(int id, [FromForm] int selectedShipperId)
        {
            _logger.LogInformation("Selected Shipper ID: {SelectedShipperId}", selectedShipperId);

            if (selectedShipperId == 0)
            {
                TempData["ErrorMessage"] = "Please select a valid shipper.";
                return RedirectToAction("HandleOrder", new { id });
            }

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

                    _context.Entry(order).State = EntityState.Detached;
                    _context.Orders.Attach(order);

                    // Verify if selectedShipperId is valid
                    var shipperExists = await _context.Shippers.AnyAsync(s => s.ShipperId == selectedShipperId);
                    if (!shipperExists)
                    {
                        TempData["ErrorMessage"] = "The selected shipper does not exist.";
                        return RedirectToAction("HandleOrder", new { id = order.OrderId });
                    }

                    var productIds = order.OrderDetails.Select(od => od.ProductId).ToHashSet();
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

                        order.Status = OrderStatus.Completed;
                        order.HandlingStartTime = null;
                        order.ShippedDate = DateTime.Now;
                        order.ShipVia = selectedShipperId;
                        _context.Entry(order).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        LogAndNotifyOrderCompletion(order);

                        TempData["SuccessMessage"] = "Order has been successfully completed.";
                        return RedirectToAction("PendingOrders");
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

                    if (order != null)
                    {
                        _context.Entry(order).Reload();

                        if (order.Status != OrderStatus.BeingHandled)
                        {
                            TempData["ErrorMessage"] = "Order quantities can only be updated for orders in the 'Being Reviewed' status.";
                            return RedirectToAction("HandleOrder", new { id = order.OrderId });
                        }
                    }

                    // Retry the update operation
                    order.Status = OrderStatus.Completed;
                    order.HandlingStartTime = null;

                    _context.Entry(order).State = EntityState.Modified;
                    try
                    {
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        LogAndNotifyOrderCompletion(order);

                        TempData["SuccessMessage"] = "Order has been successfully completed.";
                        return RedirectToAction("PendingOrders");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = "Unable to complete the order due to a concurrency conflict. Please try again.";
                        return RedirectToAction("HandleOrder", new { id = order.OrderId });
                    }
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


      


        private void LogAndNotifyOrderCompletion(Order order)
        {
            _ = SendOrderCompletionNotification(order);
            _logger.LogInformation("Order with ID: {OrderId} has been completed.", order.OrderId);
        }










        [HttpPost]
        [Authorize(Roles = "Manager,Employee")]
        public async Task<IActionResult> UpdateOrderQuantities(int id, List<UpdatedOrderDetailViewModel> updatedOrderDetails)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatus.BeingHandled)
            {
                TempData["ErrorMessage"] = "Order quantities can only be updated for orders in the 'Being Reviewed' status.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }



            var currentUser = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
            _logger.LogInformation("EmployeeId: {EmployeeId}", currentUser?.Value);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Unable to retrieve the current employee's ID.";
                return RedirectToAction("PendingOrders");
            }
            else
            {
                var currentEmployeeId = _context.Users.Where(u => u.Id == currentUser.Value).Select(u => u.EmployeeId).FirstOrDefault();
                _logger.LogInformation("EmployeeId: {EmployeeId}", currentEmployeeId.Value);


                if (order.EmployeeId != currentEmployeeId)
                {
                    TempData["ErrorMessage"] = "You are not authorized to update this order.";
                    return RedirectToAction("HandleOrder", new { id = order.OrderId });
                }
            }
            foreach (var updatedOrderDetail in updatedOrderDetails)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == updatedOrderDetail.ProductId);
                if (orderDetail != null)
                {
                    var product = orderDetail.Product;
                    if (product != null)
                    {
                        _context.Entry(product).Property("Timestamp").OriginalValue = product.Timestamp;
                    }
                }
            }

            try
            {
                UpdateOrderDetails(order, updatedOrderDetails);
                await _context.SaveChangesAsync();


                await _context.SaveChangesAsync();

                _logger.LogInformation("Order quantities updated for order with ID: {OrderId}", order.OrderId);
                TempData["SuccessMessage"] = "Order quantities updated successfully.";
                TempData["UpdateSuccess"] = true;
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



        private void UpdateOrderDetails(Order order, List<UpdatedOrderDetailViewModel> updatedOrderDetails)
        {
            foreach (var updatedOrderDetail in updatedOrderDetails)
            {
                var orderDetail = order.OrderDetails.FirstOrDefault(od => od.ProductId == updatedOrderDetail.ProductId);
                if (orderDetail != null)
                {
                    var product = orderDetail.Product;
                    if (product != null)
                    {
                        int quantityChange = updatedOrderDetail.Quantity - orderDetail.Quantity;
                        orderDetail.Quantity = (short)updatedOrderDetail.Quantity;

                        if (product != null)
                        {
                            product.ReservedStock += quantityChange;
                            _context.Update(product);
                        }
                    }
                }
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

            if (order.Status != OrderStatus.BeingHandled)
            {
                TempData["ErrorMessage"] = "Order quantities can only be updated for orders in the 'Being Reviewed' status.";
                return RedirectToAction("HandleOrder", new { id = order.OrderId });
            }

            var viewModel = order.OrderDetails.Select(od => new UpdatedOrderDetailViewModel
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                Product = od.Product,
                Order = order
            }).ToList();

            return View(viewModel);
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

            if (order.Status != OrderStatus.BeingHandled)
            {
                order.Status = OrderStatus.BeingHandled;
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
                await _context.SaveChangesAsync();

                RevertProductStocks(order, currentProducts);
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

        private void RevertProductStocks(Order order, List<Product> currentProducts)
        {
            foreach (var orderDetail in order.OrderDetails)
            {
                var product = currentProducts.FirstOrDefault(p => p.ProductId == orderDetail.ProductId);
                if (product != null)
                {
                    product.ReservedStock -= orderDetail.Quantity;
                }
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

            order.Status = OrderStatus.Cancelled;
            order.HandlingStartTime = null;
            order.CancellationReason = cancellationReason;

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
                .Where(o => o.Status == OrderStatus.BeingHandled &&
                            o.HandlingStartTime < DateTime.Now.AddMinutes(-10))
                .ToList();

            foreach (var order in staleOrders)
            {
                order.Status = OrderStatus.Pending;
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


            order.Status = OrderStatus.Pending;

            order.HandlingStartTime = null;

            await _context.SaveChangesAsync();

            // Redirect to the pending orders page or any other appropriate page
            return RedirectToAction("PendingOrders", "Orders");
        }







        private async Task SendOrderCompletionNotification(Order order)
        {
            try
            {
                var emailContent = $"Dear customer,\n\nYour order with ID: {order.OrderId} has been completed.\n\nThank you for your purchase!";
                var response = await _emailService.SendSimpleMessageAsync(order.GuestEmail, "Order Completed", emailContent);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Order completion notification sent for order with ID: {OrderId}", order.OrderId);
                }
                else
                {
                    _logger.LogError("Failed to send order completion notification for order with ID: {OrderId}. Response: {Response}", order.OrderId, response.Content);
                }
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
                var emailContent = $"Dear customer,\n\nYour order with ID: {order.OrderId} has been cancelled.\n\nWe apologize for any inconvenience this may cause.";
                var response = await _emailService.SendSimpleMessageAsync(order.GuestEmail, "Order Cancelled", emailContent);

                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Order cancellation notification sent for order with ID: {OrderId}", order.OrderId);
                }
                else
                {
                    _logger.LogError("Failed to send order cancellation notification for order with ID: {OrderId}. Response: {Response}", order.OrderId, response.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order cancellation notification for order with ID: {OrderId}", order.OrderId);
            }
        }





    }
}
