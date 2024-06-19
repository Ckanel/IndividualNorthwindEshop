using CommonData.Data;
using CommonData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Extract
{
    public class OrderRepository
    {
        private readonly MasterContext _context;
        
        public OrderRepository(MasterContext context)
        {
            _context = context;
        }
       //extract order
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        //extract orderdetails
        public async Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        // extra orders pagination
        public async Task<IEnumerable<Order>> GetPagedOrdersAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _context.Orders
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        // extract orderdetails page
        public async Task<IEnumerable<Order>> GetPagedOrdersWithDetailsAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Employee)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        // Extract orders within a specific date range
        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        //order details within a specific date range
        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.OrderDetails
                    .Include(od => od.Order)
                    .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<OrderDetail>();
            }
        }



        // Extract orders for a specific customer
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        // Extract orders that contain products from a specific category
        public async Task<IEnumerable<Order>> GetOrdersByProductCategoryAsync(int categoryId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Category)
                    .Where(o => o.OrderDetails.Any(od => od.Product.CategoryId == categoryId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Order>();
            }
        }
        // Extract customers along with their orders
        public async Task<IEnumerable<Customer>> GetCustomersWithOrdersAsync()
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Orders)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Customer>();
            }
        }
        // Extract products along with their categories and suppliers
        public async Task<IEnumerable<Product>> GetProductsWithDetailsAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Product>();
            }
        }
        public async Task<IEnumerable<Employee>> GetEmployeeAsyncDetails()
        {
            try
            {
                return await _context.Employees
                    
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<Employee>();
            }
        }

    }
}
