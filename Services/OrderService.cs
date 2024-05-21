using CommonData.Data;
using CommonData.Models;

public class OrderService
{
    private readonly MasterContext _context;

    public OrderService(MasterContext context)
    {
        _context = context;
    }

    public Order CreateOrder(string customerId, CheckoutModel model, List<CartItem> cartItems)
    {
        // Create a new order
        var order = new Order
        {
            OrderDate = DateTime.Now,
            CustomerId = customerId,
            ShipName = model.CustomerName,
            ShipAddress = model.Address,
            ShipCity = model.City,
            ShipCountry = model.Country,
            ShipPostalCode = model.PostalCode,
            GuestEmail = model.GuestEmail
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        return order;
    }

    public void CreateOrderDetails(Order order, List<CartItem> cartItems)
    {
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
    }

   




}

