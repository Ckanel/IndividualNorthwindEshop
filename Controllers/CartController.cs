using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IndividualNorthwindEshop.Data;
using IndividualNorthwindEshop.Models;
using System.Security.Claims;
using IndividualNorthwindEshop.Services;
namespace IndividualNorthwindEshop.Controllers
{
    public class CartController : Controller
    {
        private readonly MasterContext _context;

        public CartController(MasterContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var cart = GetOrCreateCart();
            var cartItems = cart.CartItems.ToList();
            Console.WriteLine($"Cart ID: {cart.CartId}");
            Console.WriteLine($"Number of cart items: {cartItems.Count}");
            return View(cartItems);
        }





        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var cart = GetOrCreateCart();
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    cart.CartItems.Add(cartItem);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

       
        private Cart GetOrCreateCart()
        {
            var customerId = User.FindFirstValue("CustomerId");
            Console.WriteLine($"CustomerId: {customerId}");

            if (customerId != null)
            {
                // User is authenticated, retrieve the cart associated with the customer
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.CustomerId == customerId);

                if (cart != null)
                {
                    return cart;
                }
            }
            else
            {
                // User is not authenticated, retrieve the cart from the session
                var cartId = HttpContext.Session.GetInt32("CartId");

                if (cartId.HasValue)
                {
                    var cart = _context.Carts
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.CartId == cartId.Value);

                    if (cart != null)
                    {
                        return cart;
                    }
                }
            }

            // If no cart is found, create a new cart
            var newCart = new Cart
            {
                CartItems = new List<CartItem>() // Initialize an empty CartItems collection
            };
            _context.Carts.Add(newCart);
            _context.SaveChanges();

            // Store the cart ID in the session
            HttpContext.Session.SetInt32("CartId", newCart.CartId);

            return newCart;
        }






        [HttpPost]
        public IActionResult UpdateCartQuantity(int productId, int quantity)
        {
            var cart = GetOrCreateCart();
            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetOrCreateCart();
            var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                cart.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }


}
