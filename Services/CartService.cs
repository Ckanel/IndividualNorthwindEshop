using CommonData.Data;
using CommonData.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;



public class CartService
{
    private readonly MasterContext _context;

    public CartService(MasterContext context)
    {
        _context = context;
    }

    public Cart GetOrCreateCart(string customerId, HttpContext httpContext)
    {
        Cart cart;

        if (customerId != null)
        {
            // User is authenticated, retrieve the cart associated with the customer
            cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.CustomerId == customerId);

            

// If no cart is found, create a new cart
if (cart == null)
            {
                var newCart = new Cart
                {
                    CartItems = new List<CartItem>() // Initialize an empty CartItems collection
                };

                // For authenticated users, set the CustomerId
                if (customerId != null)
                {
                    newCart.CustomerId = customerId;
                }

                _context.Carts.Add(newCart);
                _context.SaveChanges();

                return newCart;
            }
        }
        else
        {
            // User is not authenticated, retrieve the cart from the session
            var cartId = httpContext.Session.GetInt32("CartId");

            if (cartId.HasValue)
            {
                cart = _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.CartId == cartId.Value);

                if (cart == null)
                {
                    // Create a new cart for the unauthenticated user
                    cart = new Cart { CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                    _context.SaveChanges();

                    // Store the cart ID in the session
                    httpContext.Session.SetInt32("CartId", cart.CartId);
                }
            }
            else
            {
                // Create a new cart for the unauthenticated user
                cart = new Cart { CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                _context.SaveChanges();

                // Store the cart ID in the session
                httpContext.Session.SetInt32("CartId", cart.CartId);
            }
        }

        return cart;
    }

    public void AddToCart(Cart cart, int productId, int quantity)
    {
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
    }

    public void UpdateCartQuantity(Cart cart, int productId, int quantity)
    {
        var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            _context.SaveChanges();
        }
    }

    public void RemoveFromCart(Cart cart, int productId)
    {
        var cartItem = cart.CartItems.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem != null)
        {
            cart.CartItems.Remove(cartItem);
            _context.SaveChanges();
        }
    }
}

