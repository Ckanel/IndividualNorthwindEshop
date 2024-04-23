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
using Microsoft.AspNetCore.Cors.Infrastructure;
namespace IndividualNorthwindEshop.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _cartService.GetOrCreateCart(customerId, HttpContext);
            var cartItems = cart.CartItems.ToList();

            Console.WriteLine($"Cart ID: {cart.CartId}");
            Console.WriteLine($"Number of cart items: {cartItems.Count}");

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _cartService.GetOrCreateCart(customerId, HttpContext);

            _cartService.AddToCart(cart, productId, quantity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCartQuantity(int productId, int quantity)
        {
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _cartService.GetOrCreateCart(customerId, HttpContext);

            _cartService.UpdateCartQuantity(cart, productId, quantity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var customerId = User.FindFirstValue("CustomerId");
            var cart = _cartService.GetOrCreateCart(customerId, HttpContext);

            _cartService.RemoveFromCart(cart, productId);

            return RedirectToAction("Index");
        }
    }


    }
