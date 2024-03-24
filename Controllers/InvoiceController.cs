using IndividualNorthwindEshop.Data;
using IndividualNorthwindEshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndividualNorthwindEshop.Controllers
{
    [Authorize(Roles = "Manager,Employee")]
    public class InvoiceController : Controller
    {
        private readonly MasterContext _context;
        
        public InvoiceController(MasterContext context)
        {
            _context = context;
        }

        // GET: Invoices/SearchCustomers
        public IActionResult SearchCustomer()
        {
            return View();
        }

        // POST: Invoices/SearchCustomers
        [HttpPost]
        public IActionResult SearchCustomer(string searchTerm)
        {
            var customers = _context.Customers
                .Where(c => c.CompanyName.Contains(searchTerm) || c.CustomerId.ToString().Contains(searchTerm))
                .ToList();

            return View("CustomerSearchResults", customers);
        }

        

        public IActionResult CustomerInvoices(string customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new CustomerOrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerId = o.CustomerId,
                    CompanyName = o.Customer.CompanyName,
                    CustomerEmail = o.GuestEmail, 
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailViewModel
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        ExtendedPrice = od.Quantity * od.UnitPrice
                    }).ToList()
                })
                .ToList();

            return View(orders);
        }










    }
}
