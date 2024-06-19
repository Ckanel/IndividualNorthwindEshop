using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using ETL.Transform.TransformedModels;

namespace ETL.Transform.DataAccess
{
    public class ETLDataExportService
    {
        private readonly string _connectionString;

        public ETLDataExportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to retrieve CustomerInsights
        public async Task<IEnumerable<CustomerInsights>> GetCustomerInsightsAsync()
        {
            var insights = new List<CustomerInsights>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM CustomerInsights";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        insights.Add(new CustomerInsights
                        {
                            CustomerId = reader["CustomerId"].ToString(),
                            TotalOrders = (int)reader["TotalOrders"],
                            TotalSpent = (decimal)reader["TotalSpent"]
                        });
                    }
                }
            }

            return insights;
        }

        // Method to retrieve TransformedOrders
        public async Task<IEnumerable<TransformedOrder>> GetTransformedOrdersAsync()
        {
            var orders = new List<TransformedOrder>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM TransformedOrder";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(new TransformedOrder
                        {
                            OrderId = (int)reader["OrderId"],
                            OrderDate = (DateTime)reader["OrderDate"],
                            ShippedDate = reader["ShippedDate"] as DateTime?,
                            ShipName = reader["ShipName"].ToString(),
                            ShipAddress = reader["ShipAddress"].ToString(),
                            ShipCity = reader["ShipCity"].ToString(),
                            ShipRegion = reader["ShipRegion"].ToString(),
                            ShipPostalCode = reader["ShipPostalCode"].ToString(),
                            ShipCountry = reader["ShipCountry"].ToString(),
                            CustomerId = reader["CustomerId"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            CustomerContact = reader["CustomerContact"].ToString(),
                            TotalOrderAmount = (decimal)reader["TotalOrderAmount"],
                            EmployeeId = reader["EmployeeId"] as int?
                        });
                    }
                }
            }

            return orders;
        }

        // Method to retrieve TransformedOrderDetails
        public async Task<IEnumerable<TransformedOrderDetail>> GetTransformedOrderDetailsAsync()
        {
            var orderDetails = new List<TransformedOrderDetail>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM TransformedOrderDetail";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orderDetails.Add(new TransformedOrderDetail
                        {
                            OrderDetailId = (int)reader["OrderDetailId"],
                            ProductId = (int)reader["ProductId"],
                            UnitPrice = (decimal)reader["UnitPrice"],
                            Quantity = (short)reader["Quantity"],
                            Discount = (float)reader["Discount"],
                            TotalAmount = (decimal)reader["TotalAmount"]
                        });
                    }
                }
            }

            return orderDetails;
        }

        // Method to retrieve SalesForecasts
        public async Task<IEnumerable<SalesForecast>> GetSalesForecastsAsync()
        {
            var forecasts = new List<SalesForecast>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM SalesForecast";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        forecasts.Add(new SalesForecast
                        {
                            Year = (int)reader["Year"],
                            Month = (int)reader["Month"],
                            PredictedSales = (decimal)reader["PredictedSales"]
                        });
                    }
                }
            }

            return forecasts;
        }

        // Method to retrieve SalesData
        public async Task<IEnumerable<SalesData>> GetSalesDataAsync()
        {
            var salesData = new List<SalesData>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM SalesData";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        salesData.Add(new SalesData
                        {
                            Year = (int)reader["Year"],
                            Month = (int)reader["Month"],
                            TotalSales = (decimal)reader["TotalSales"]
                        });
                    }
                }
            }

            return salesData;
        }

        // Method to retrieve ProductPerformances
        public async Task<IEnumerable<ProductPerformance>> GetProductPerformancesAsync()
        {
            var productPerformances = new List<ProductPerformance>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM ProductPerformance";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        productPerformances.Add(new ProductPerformance
                        {
                            ProductId = (int)reader["ProductId"],
                            TotalQuantitySold = (int)reader["TotalQuantitySold"],
                            TotalAmountSold = (decimal)reader["TotalAmountSold"],
                            TotalRevenue = (decimal)reader["TotalRevenue"],
                            ProductName = (string)reader["ProductName"]
                        });
                    }
                }
            }

            return productPerformances;
        }

        // Method to retrieve DiscountEffectiveness
        public async Task<IEnumerable<DiscountEffectiveness>> GetDiscountEffectivenessAsync()
        {
            var discountEffectiveness = new List<DiscountEffectiveness>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM DiscountEffectiveness";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        discountEffectiveness.Add(new DiscountEffectiveness
                        {
                            DiscountRate = (float)reader.GetDouble(reader.GetOrdinal("DiscountRate")),
                            TotalSales = (decimal)reader["TotalSales"]
                        });
                    }
                }
            }

            return discountEffectiveness;
        }

        // Method to retrieve EmployeePerformance
        public async Task<IEnumerable<EmployeePerformance>> GetEmployeePerformancesAsync()
        {
            var employeePerformances = new List<EmployeePerformance>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT * FROM EmployeePerformance";

                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeePerformances.Add(new EmployeePerformance
                        {
                            EmployeeId = (int)reader["EmployeeId"],
                            OrdersHandled = (int)reader["OrdersHandled"],
                            AverageHandlingTime = (double)reader["AverageHandlingTime"],
                            EmployeeFirstName = reader["EmployeeFirstName"].ToString(),
                            EmployeeLastName = reader["EmployeeLastName"].ToString()
                        });
                    }
                }
            }

            return employeePerformances;
        }
    }
}