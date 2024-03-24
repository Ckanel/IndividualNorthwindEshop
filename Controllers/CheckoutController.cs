using IndividualNorthwindEshop.Data;
using IndividualNorthwindEshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace IndividualNorthwindEshop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MasterContext _context;

        public CheckoutController(MasterContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Retrieve the user's cart
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _context.Carts.FirstOrDefault(c => c.CustomerId == customerId);
            Debug.WriteLine($"CustomerId: {customerId}");
            Debug.WriteLine($"Cart: {cart}");

            if (cart == null)
            {
                // Handle the case when the user doesn't have a cart
                return RedirectToAction("Index", "Cart");
            }

            // Retrieve the user's shopping cart items
            var cartItems = _context.CartItems.Include(item => item.Product)
                .Where(item => item.CartId == cart.CartId)
                .ToList();

            // Pass the cart items to the view
            return View(cartItems);
        }

    
        [HttpPost]
        public IActionResult ProcessOrder(CheckoutModel model)
        {
            Cart cart = null;
            var customerId = User.FindFirstValue("CustomerId");

            if (customerId != null)
            {
                // User is authenticated, retrieve the cart associated with the customer
                cart = _context.Carts.FirstOrDefault(c => c.CustomerId == customerId);
            }
            else
            {
                // User is not authenticated, retrieve the cart from the session
                var cartId = HttpContext.Session.GetInt32("CartId");
                if (cartId.HasValue)
                {
                    cart = _context.Carts.FirstOrDefault(c => c.CartId == cartId.Value);
                }
            }

            if (cart == null)
            {
                // Handle the case when the user doesn't have a cart
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = _context.CartItems.Include(item => item.Product)
                .Where(item => item.CartId == cart.CartId)
                .ToList();

            // Create a new order
            var order = new Order
            {
                CustomerId = customerId, // Set the CustomerId to null for unauthenticated users
                OrderDate = DateTime.Now,
                GuestEmail = model.GuestEmail,
                ShipName = model.CustomerName,
                ShipAddress = model.Address,
                ShipCity = model.City,
                ShipCountry = model.Country,
                ShipPostalCode = model.PostalCode,
                // Set other order properties based on the model
            };

            // Add the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Create order items and associate them with the order
            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = (short)cartItem.Quantity,
                    UnitPrice = (decimal)cartItem.Product.UnitPrice
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            // Clear the user's shopping cart
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            // Clear the cart ID from the session for unauthenticated users
            if (customerId == null)
            {
                HttpContext.Session.Remove("CartId");
            }

            // Redirect to the order confirmation page
            return RedirectToAction("Confirmation", new { orderId = order.OrderId });
        }



        public IActionResult Confirmation(int orderId)
        {
            // Retrieve the order from the database
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            // Pass the order to the view
            return View(order);
        }
    }
}
