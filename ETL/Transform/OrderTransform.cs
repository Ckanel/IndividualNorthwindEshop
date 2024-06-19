using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CommonData.Models;
using ETL.Transform.TransformedModels;

namespace ETL.Transform
{
    public class OrderTransform  
    {
        public IEnumerable<TransformedOrder> TransformOrders(IEnumerable<Order> orders)
        {
            foreach (var order in orders)
            {
                order.ShipName = order.ShipName ?? "N/A";
                order.ShipAddress = order.ShipAddress ?? "N/A";
                order.ShipCity = order.ShipCity ?? "N/A";
                order.ShipRegion = order.ShipRegion ?? "N/A";
                order.CustomerId = order.CustomerId ?? "Guest";
                order.ShipPostalCode = order.ShipPostalCode ?? "N/A";
                order.ShipCountry = order.ShipCountry ?? "N/A";
                
                if (order.EmployeeId == null)
                {
                    Debug.WriteLine($"Warning: EmployeeId is null for OrderId={order.OrderId}");
                }
                else { Debug.WriteLine($"Warning: EmployeeId is = { order.EmployeeId}"); }
                Debug.WriteLine($"Before CleanString: ShipName={order.ShipName}, ShipAddress={order.ShipAddress}, ShipCity={order.ShipCity}");

                order.ShipName = CleanString(order.ShipName);
                order.ShipAddress = CleanString(order.ShipAddress);
                order.ShipCity = CleanString(order.ShipCity);
                order.ShipRegion = CleanString(order.ShipRegion);
                order.ShipPostalCode = CleanString(order.ShipPostalCode);
                order.ShipCountry = CleanString(order.ShipCountry);
               
                Debug.WriteLine($"After CleanString: ShipName={order.ShipName}, ShipAddress={order.ShipAddress}, ShipCity={order.ShipCity}");
                var customer = order.Customer;
                var transformedOrder = new TransformedOrder
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    ShippedDate = order.ShippedDate,
                    ShipName = order.ShipName,
                    ShipAddress = order.ShipAddress,
                    ShipCity = order.ShipCity,
                    ShipRegion = order.ShipRegion,
                    ShipPostalCode = order.ShipPostalCode,
                    ShipCountry = order.ShipCountry,
                    CustomerId = order.CustomerId ,
                    CustomerName = customer?.CompanyName ?? "N/A",
                    CustomerContact = customer?.ContactName ?? "N/A",
                    TotalOrderAmount = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)),
                    EmployeeId = order.EmployeeId ?? -1
                 
                };

                yield return transformedOrder;
            }
        }
        
        public string CleanString(string input)  
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Debug.WriteLine("String was null or whitespace, returning 'N/A'");
                return "N/A";
            }

            if (input.Equals("N/A", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Input was 'N/A', no transformation needed");
                return "N/A";
            }
            if (input.Equals("NA", StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine("Input was 'NA', no transformation needed");
                return "N/A";
            }

            input = input.Trim();
            input = Regex.Replace(input, @"[^a-zA-Z0-9\s]", string.Empty);
            input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());

            Debug.WriteLine($"Cleaned string: {input}");
            return input;
        }

        public IEnumerable<Product> TransformProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                product.ProductName = product.ProductName ?? "N/A";
                product.QuantityPerUnit = product.QuantityPerUnit ?? "N/A";

                product.ProductName = CleanString(product.ProductName);
                product.QuantityPerUnit = CleanString(product.QuantityPerUnit);

                product.UnitPrice = HandleProductUnitPrice(product.UnitPrice);
                product.UnitsInStock = HandleProductUnitsInStock(product.UnitsInStock, product.ReorderLevel);
                product.UnitsOnOrder = HandleProductUnitsOnOrder(product.UnitsOnOrder);
                product.Discontinued = product.Discontinued;
                yield return product;
            }
        }

        public decimal HandleProductUnitPrice(decimal? unitPrice)
        {
            try
            {
                if (unitPrice < 0)
                {
                    throw new ArgumentException("UnitPrice cannot be negative.");
                }
                return Math.Round(unitPrice ?? 0, 2);
            }
            catch (Exception ex)
            {
                LogError("HandleProductUnitPrice", ex);
                return 0;
            }
        }

        public short HandleProductUnitsInStock(short? unitsInStock, short? reorderLevel)
        {
            try
            {
                if (unitsInStock < 0)
                {
                    throw new ArgumentException("UnitsInStock cannot be negative.");
                }
                return unitsInStock ?? 0;
            }
            catch (Exception ex)
            {
                LogError("HandleProductUnitsInStock", ex);
                return 0;
            }
        }

        private void LogError(string methodName, Exception ex)
        {
            Debug.WriteLine($"Error in {methodName}: {ex.Message}");
        }

        public short HandleProductUnitsOnOrder(short? unitsOnOrder)
        {
            try
            {
                if (unitsOnOrder < 0)
                {
                    throw new ArgumentException("UnitsOnOrder cannot be negative.");
                }
                return (short)Math.Min(unitsOnOrder ?? (short)0, (short)1000);
            }
            catch (Exception ex)
            {
                LogError("HandleProductUnitsOnOrder", ex);
                return 0;
            }
        }
        public IEnumerable<Employee> TransformEmployees (IEnumerable<Employee> employees)
        {
            foreach (var employee in employees)
            {
                employee.FirstName = employee.FirstName ?? "N/A";
                employee.LastName = employee.LastName ?? "N/A";
                employee.Title = employee.Title ?? "N/A";
                employee.TitleOfCourtesy = employee.TitleOfCourtesy ?? "N/A";
                employee.Address = employee.Address ?? "N/A";
                employee.City = employee.City ?? "N/A";
                employee.Region = employee.Region ?? "N/A";
                employee.PostalCode = employee.PostalCode ?? "N/A";
                employee.Country = employee.Country ?? "N/A";
                employee.HomePhone = employee.HomePhone ?? "N/A";
                employee.Extension = employee.Extension ?? "N/A";
                employee.Notes = employee.Notes ?? "N/A";

                employee.FirstName = CleanString(employee.FirstName);
                employee.LastName = CleanString(employee.LastName);
                employee.Title = CleanString(employee.Title);
                employee.TitleOfCourtesy = CleanString(employee.TitleOfCourtesy);
                employee.Address = CleanString(employee.Address);
                employee.City = CleanString(employee.City);
                employee.Region = CleanString(employee.Region);
                employee.PostalCode = CleanString(employee.PostalCode);
                employee.Country = CleanString(employee.Country);
                employee.HomePhone = CleanString(employee.HomePhone);
                employee.Extension = CleanString(employee.Extension);
                employee.Notes = CleanString(employee.Notes);
                yield return employee;
            }
        }   
        public IEnumerable<TransformedOrderDetail> TransformOrderDetails(IEnumerable<OrderDetail> orderDetails)
        {
            foreach (var orderDetail in orderDetails)
            {
                float discount = orderDetail.Discount;

                orderDetail.UnitPrice = HandleOrderDetailUnitPrice(orderDetail.UnitPrice);
                orderDetail.Quantity = HandleOrderDetailQuantity(orderDetail.Quantity);

                decimal totalAmount = CalculateOrderDetailTotal(orderDetail.UnitPrice, orderDetail.Quantity, discount);

                yield return new TransformedOrderDetail
                {
                    OrderDetailId = orderDetail.OrderId,
                    ProductId = orderDetail.ProductId,
                    UnitPrice = orderDetail.UnitPrice,
                    Quantity = orderDetail.Quantity,
                    Discount = discount,
                    TotalAmount = totalAmount
                };
            }
        }

        public decimal HandleOrderDetailUnitPrice(decimal? unitPrice)
        {
            try
            {
                if (unitPrice < 0)
                {
                    throw new ArgumentException("UnitPrice cannot be negative.");
                }
                return Math.Round(unitPrice ?? 0, 2);
            }
            catch (Exception ex)
            {
                LogError("HandleOrderDetailUnitPrice", ex);
                return 0;
            }
        }

        public short HandleOrderDetailQuantity(short? quantity)
        {
            try
            {
                if (quantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative.");
                }
                return quantity ?? 0;
            }
            catch (Exception ex)
            {
                LogError("HandleOrderDetailQuantity", ex);
                return 0;
            }
        }

        public decimal CalculateOrderDetailTotal(decimal unitPrice, short quantity, float discount)
        {
            return (unitPrice * quantity) * (1 - (decimal)discount);
        }

        public IEnumerable<Customer> TransformCustomers(IEnumerable<Customer> customers)
        {
            foreach (var customer in customers)
            {
                customer.CompanyName = customer.CompanyName ?? "N/A";
                customer.ContactName = customer.ContactName ?? "N/A";
                customer.ContactTitle = customer.ContactTitle ?? "N/A";
                customer.Address = customer.Address ?? "N/A";
                customer.City = customer.City ?? "N/A";
                customer.Region = customer.Region ?? "N/A";
                customer.PostalCode = customer.PostalCode ?? "N/A";
                customer.Country = customer.Country ?? "N/A";
                customer.Phone = customer.Phone ?? "N/A";
                customer.Fax = customer.Fax ?? "N/A";

                customer.CompanyName = CleanString(customer.CompanyName);
                customer.ContactName = CleanString(customer.ContactName);
                customer.ContactTitle = CleanString(customer.ContactTitle);
                customer.Address = CleanString(customer.Address);
                customer.City = CleanString(customer.City);
                customer.Region = CleanString(customer.Region);
                customer.PostalCode = CleanString(customer.PostalCode);
                customer.Country = CleanString(customer.Country);
                customer.Phone = CleanString(customer.Phone);
                customer.Fax = CleanString(customer.Fax);

                yield return customer;
            }
        }

        public IEnumerable<Order> FilterOrdersByDateRange(IEnumerable<Order> orders, DateTime startDate, DateTime endDate)
        {
            return orders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);
        }

        public IEnumerable<Order> SortOrdersByDate(IEnumerable<Order> orders, bool ascending = true)
        {
            return ascending ? orders.OrderBy(o => o.OrderDate) : orders.OrderByDescending(o => o.OrderDate);
        }

        public IEnumerable<IGrouping<string, Order>> GroupOrdersByCustomer(IEnumerable<Order> orders)
        {
            return orders.GroupBy(o => o.CustomerId);
        }

        public IEnumerable<IGrouping<string, Order>> GroupOrdersByRegion(IEnumerable<Order> orders)
        {
            return orders.GroupBy(o => o.ShipRegion);
        }

        
        public IEnumerable<EmployeePerformance> CalculateEmployeePerformance(IEnumerable<Order> orders, IEnumerable<Employee> employees)
        {
            var transformedOrders = TransformOrders(orders);
            var transformedEmployees = TransformEmployees(employees).ToDictionary(e => e.EmployeeId);

            return transformedOrders.GroupBy(o => o.EmployeeId)
                .Select(g =>
                {
                    var employeeId = g.Key ?? -1;
                    string firstName, lastName;

                    if (employeeId > 0 && transformedEmployees.TryGetValue(employeeId, out var employee))
                    {
                        firstName = employee.FirstName;
                        lastName = employee.LastName;
                    }
                    else
                    {
                        firstName = "Unhandled";
                        lastName = "Orders";
                    }

                    return new EmployeePerformance
                    {
                        EmployeeId = employeeId,
                        OrdersHandled = g.Count(),
                        AverageHandlingTime = g.Average(o => (o.ShippedDate - o.OrderDate)?.TotalDays ?? 0),
                        EmployeeFirstName = firstName,
                        EmployeeLastName = lastName
                    };
                })
                .ToList();
        }

        public IEnumerable<ProductPerformance> IdentifyBestSellingProducts(IEnumerable<Order> orders, IEnumerable<Product> products)
        {
            
            var transformedProducts = TransformProducts(products).ToDictionary(p => p.ProductId, p => p.ProductName);

            var transformedOrderDetails = orders.SelectMany(order => order.OrderDetails);

            return transformedOrderDetails.GroupBy(od => od.ProductId)
                .Select(g => new ProductPerformance
                {
                    ProductId = g.Key,
                    ProductName = transformedProducts.ContainsKey(g.Key) ? transformedProducts[g.Key] : "N/A", 
                    TotalQuantitySold = g.Sum(od => od.Quantity),
                    TotalAmountSold = g.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)),
                    TotalRevenue = g.Sum(od => od.UnitPrice * od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .ToList();
        }

      









        //public IEnumerable<ProductPerformance> IdentifyBestSellingProducts(IEnumerable<Order> orders)
        //{
        //    var transformedOrderDetails = TransformOrderDetails(orders.SelectMany(order => order.OrderDetails));
        //    return transformedOrderDetails.GroupBy(od => od.ProductId)
        //        .Select(g => new ProductPerformance
        //        {
        //            ProductId = g.Key,
        //            TotalQuantitySold = g.Sum(od => od.Quantity),
        //            TotalAmountSold = g.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)od.Discount)),
        //            TotalRevenue = g.Sum(od => od.UnitPrice * od.Quantity)
        //        })
        //        .OrderByDescending(p => p.TotalQuantitySold)
        //        .ToList();
        //}

        public IEnumerable<CustomerInsights> CalculateCustomerInsights(IEnumerable<Order> orders)
        {
            var transformedOrders = TransformOrders(orders);
            return transformedOrders.GroupBy(o => o.CustomerId)
                .Select(g => new CustomerInsights
                {
                    CustomerId = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalOrderAmount)
                })
                .OrderByDescending(ci => ci.TotalSpent)
                .ToList();
        }

        public IEnumerable<SalesData> CalculateTotalSales(IEnumerable<Order> orders)
        {
            var transformedOrders = TransformOrders(orders);
            return transformedOrders.GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new SalesData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSales = g.Sum(o => o.TotalOrderAmount)
                })
                .ToList();
        }
        public IEnumerable<SalesForecast> PredictSales(IEnumerable<SalesData> salesData, int monthsForAverage = 3)
        {
            // Order sales data by year and month
            var salesPerMonth = salesData
                .OrderBy(s => s.Year).ThenBy(s => s.Month)
                .ToList();

            var forecasts = new List<SalesForecast>();
            for (int i = monthsForAverage; i < salesPerMonth.Count; i++)
            {
                var averageSales = salesPerMonth.Skip(i - monthsForAverage)
                    .Take(monthsForAverage)
                    .Average(s => s.TotalSales);
                forecasts.Add(new SalesForecast
                {
                    Year = salesPerMonth[i].Year,
                    Month = salesPerMonth[i].Month,
                    PredictedSales = averageSales
                });
            }

            return forecasts;
        }

        public IEnumerable<DiscountEffectiveness> AnalyzeDiscountEffectiveness(IEnumerable<Order> orders)
        {
            var transformedOrderDetails = TransformOrderDetails(orders.SelectMany(order => order.OrderDetails));
            return transformedOrderDetails.GroupBy(od => od.Discount)
                .Select(g => new DiscountEffectiveness
                {
                    DiscountRate = g.Key,
                    TotalSales = g.Sum(od => od.UnitPrice * od.Quantity * (1 - (decimal)g.Key))
                })
                .OrderByDescending(de => de.TotalSales)
                .ToList();
        }
    }
}

