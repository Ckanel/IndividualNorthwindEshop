using IndividualNorthwindEshop.Data;
using IndividualNorthwindEshop.Models;
using IndividualNorthwindEshop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace IndividualNorthwindEshop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MasterContext _context;
        private readonly CartService _cartService;
        private readonly OrderService _orderService;

        public CheckoutController(MasterContext context, CartService cartService, OrderService orderService)
        {
            _context = context;
            _cartService = cartService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _cartService.GetOrCreateCart(customerId, HttpContext);

            if (cart == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            var cartItems = cart.CartItems.ToList();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult ProcessOrder(CheckoutModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string customerId = GetCustomerId();
            Cart cart = _cartService.GetOrCreateCart(customerId, HttpContext);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            Order order = CreateOrder(customerId, model, cart.CartItems.ToList());
            

            ClearCart(customerId, cart.CartItems.ToList());

            return RedirectToAction("Confirmation", new { orderId = order.OrderId });
        }

        private string GetCustomerId()
        {
            // Retrieve the customer ID from the authenticated user or session
            return User.FindFirstValue("CustomerId");
        }

        private Order CreateOrder(string customerId, CheckoutModel model, List<CartItem> cartItems)
        {
            Order order = _orderService.CreateOrder(customerId, model, cartItems);
            _orderService.CreateOrderDetails(order, cartItems);

            return order;
        }

        private void ClearCart(string customerId, List<CartItem> cartItems)
        {
            // Clear the cart items from the database
            _context.CartItems.RemoveRange(cartItems);
            _context.SaveChanges();

            // Clear the cart ID from the session for unauthenticated users
            if (string.IsNullOrEmpty(customerId))
            {
                HttpContext.Session.Remove("CartId");
            }
        }



        public IActionResult Confirmation(int orderId)
        {
            // Retrieve the order from the database
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == orderId);

            return View(order);
        }
    }
}

