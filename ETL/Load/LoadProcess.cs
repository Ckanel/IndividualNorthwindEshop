using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using ETL.Transform.TransformedModels;
using System.Diagnostics;

namespace ETL.Load
{
    public class LoadProcess
    {
        private readonly string _connectionString;

        public LoadProcess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> LoadCustomerInsightsAsync(IEnumerable<CustomerInsights> insights)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                       
                        foreach (var insight in insights)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO CustomerInsights AS target
                        USING 
                        (
                            VALUES (@CustomerId, @TotalOrders, @TotalSpent)
                        ) AS source (CustomerId, TotalOrders, TotalSpent)
                        ON target.CustomerId = source.CustomerId
                        WHEN MATCHED THEN
                            UPDATE SET TotalOrders = source.TotalOrders, 
                                       TotalSpent = source.TotalSpent
                        WHEN NOT MATCHED THEN
                            INSERT (CustomerId, TotalOrders, TotalSpent)
                            VALUES (source.CustomerId, source.TotalOrders, source.TotalSpent);";
                            mergeCommand.Parameters.AddWithValue("@CustomerId", insight.CustomerId);
                            mergeCommand.Parameters.AddWithValue("@TotalOrders", insight.TotalOrders);
                            mergeCommand.Parameters.AddWithValue("@TotalSpent", insight.TotalSpent);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                       
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadCustomerInsightsAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadCustomerInsightsAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }
        // Load Product Performance
        public async Task<bool> LoadProductPerformanceAsync(IEnumerable<ProductPerformance> performances)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var performance in performances)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
MERGE INTO ProductPerformance AS target
USING 
(
    VALUES (@ProductId, @TotalQuantitySold, @TotalAmountSold, @TotalRevenue, @ProductName)
) AS source (ProductId, TotalQuantitySold, TotalAmountSold, TotalRevenue, ProductName)
ON target.ProductId = source.ProductId
WHEN MATCHED THEN
    UPDATE SET TotalQuantitySold = source.TotalQuantitySold, 
               TotalAmountSold = source.TotalAmountSold,
               TotalRevenue = source.TotalRevenue,
               ProductName = source.ProductName
WHEN NOT MATCHED THEN
    INSERT (ProductId, TotalQuantitySold, TotalAmountSold, TotalRevenue, ProductName)
    VALUES (source.ProductId, source.TotalQuantitySold, source.TotalAmountSold, source.TotalRevenue, source.ProductName);";

                            mergeCommand.Parameters.AddWithValue("@ProductId", performance.ProductId);
                            mergeCommand.Parameters.AddWithValue("@TotalQuantitySold", performance.TotalQuantitySold);
                            mergeCommand.Parameters.AddWithValue("@TotalAmountSold", performance.TotalAmountSold);
                            mergeCommand.Parameters.AddWithValue("@TotalRevenue", performance.TotalRevenue);
                            mergeCommand.Parameters.AddWithValue("@ProductName", performance.ProductName);

                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadProductPerformanceAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadProductPerformanceAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }


        // Load Employee Performance
        public async Task<bool> LoadEmployeePerformanceAsync(IEnumerable<EmployeePerformance> performances)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var performance in performances)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                    MERGE INTO EmployeePerformance AS target
                    USING 
                    (
                        VALUES (@EmployeeId, @OrdersHandled, @AverageHandlingTime, @EmployeeFirstName, @EmployeeLastName)
                    ) AS source (EmployeeId, OrdersHandled, AverageHandlingTime, EmployeeFirstName, EmployeeLastName)
                    ON target.EmployeeId = source.EmployeeId
                    WHEN MATCHED THEN
                        UPDATE SET OrdersHandled = source.OrdersHandled,
                                   AverageHandlingTime = source.AverageHandlingTime,
                                   EmployeeFirstName = source.EmployeeFirstName,
                                   EmployeeLastName = source.EmployeeLastName
                    WHEN NOT MATCHED THEN
                        INSERT (EmployeeId, OrdersHandled, AverageHandlingTime, EmployeeFirstName, EmployeeLastName)
                        VALUES (source.EmployeeId, source.OrdersHandled, source.AverageHandlingTime, source.EmployeeFirstName, source.EmployeeLastName);";

                            mergeCommand.Parameters.AddWithValue("@EmployeeId", performance.EmployeeId);
                            mergeCommand.Parameters.AddWithValue("@OrdersHandled", performance.OrdersHandled);
                            mergeCommand.Parameters.AddWithValue("@AverageHandlingTime", performance.AverageHandlingTime);
                            mergeCommand.Parameters.AddWithValue("@EmployeeFirstName", performance.EmployeeFirstName);
                            mergeCommand.Parameters.AddWithValue("@EmployeeLastName", performance.EmployeeLastName);

                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadEmployeePerformanceAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadEmployeePerformanceAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }

        // Load Discount Effectiveness
        public async Task<bool> LoadDiscountEffectivenessAsync(IEnumerable<DiscountEffectiveness> effectiveness)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var effect in effectiveness)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO DiscountEffectiveness AS target
                        USING 
                        (
                            VALUES (@DiscountRate, @TotalSales)
                        ) AS source (DiscountRate, TotalSales)
                        ON target.DiscountRate = source.DiscountRate
                        WHEN MATCHED THEN
                            UPDATE SET TotalSales = source.TotalSales
                        WHEN NOT MATCHED THEN
                            INSERT (DiscountRate, TotalSales)
                            VALUES (source.DiscountRate, source.TotalSales);";
                            mergeCommand.Parameters.AddWithValue("@DiscountRate", effect.DiscountRate);
                            mergeCommand.Parameters.AddWithValue("@TotalSales", effect.TotalSales);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadDiscountEffectivenessAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadDiscountEffectivenessAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }

        // Load Sales Forecast
        public async Task<bool> LoadSalesForecastAsync(IEnumerable<SalesForecast> forecasts)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var forecast in forecasts)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO SalesForecast AS target
                        USING 
                        (
                            VALUES (@Year, @Month, @PredictedSales)
                        ) AS source (Year, Month, PredictedSales)
                        ON target.Year = source.Year AND target.Month = source.Month
                        WHEN MATCHED THEN
                            UPDATE SET PredictedSales = source.PredictedSales
                        WHEN NOT MATCHED THEN
                            INSERT (Year, Month, PredictedSales)
                            VALUES (source.Year, source.Month, source.PredictedSales);";
                            mergeCommand.Parameters.AddWithValue("@Year", forecast.Year);
                            mergeCommand.Parameters.AddWithValue("@Month", forecast.Month);
                            mergeCommand.Parameters.AddWithValue("@PredictedSales", forecast.PredictedSales);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadSalesForecastAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadSalesForecastAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }
        //Load total sales
        public async Task<bool> LoadTotalSalesAsync(IEnumerable<SalesData> salesData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var data in salesData)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO SalesData AS target
                        USING 
                        (
                            VALUES (@Year, @Month, @TotalSales)
                        ) AS source (Year, Month, TotalSales)
                        ON target.Year = source.Year AND target.Month = source.Month
                        WHEN MATCHED THEN
                            UPDATE SET TotalSales = source.TotalSales
                        WHEN NOT MATCHED THEN
                            INSERT (Year, Month, TotalSales)
                            VALUES (source.Year, source.Month, source.TotalSales);";
                            mergeCommand.Parameters.AddWithValue("@Year", data.Year);
                            mergeCommand.Parameters.AddWithValue("@Month", data.Month);
                            mergeCommand.Parameters.AddWithValue("@TotalSales", data.TotalSales);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadTotalSalesAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadTotalSalesAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }
        // Load Transformed Orders
        public async Task<bool> LoadTransformedOrdersAsync(IEnumerable<TransformedOrder> orders)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var order in orders)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO TransformedOrder AS target
                        USING 
                        (
                            VALUES (@OrderId, @CustomerId, @CustomerName, @CustomerContact, @OrderDate, @ShippedDate, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry, @TotalOrderAmount, @EmployeeId)
                        ) AS source (OrderId, CustomerId, CustomerName, CustomerContact, OrderDate, ShippedDate, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, TotalOrderAmount, EmployeeId)
                        ON target.OrderId = source.OrderId
                        WHEN MATCHED THEN
                            UPDATE SET CustomerId = source.CustomerId, 
                                       CustomerName = source.CustomerName, 
                                       CustomerContact = source.CustomerContact, 
                                       OrderDate = source.OrderDate, 
                                       ShippedDate = source.ShippedDate, 
                                       ShipName = source.ShipName, 
                                       ShipAddress = source.ShipAddress, 
                                       ShipCity = source.ShipCity, 
                                       ShipRegion = source.ShipRegion, 
                                       ShipPostalCode = source.ShipPostalCode, 
                                       ShipCountry = source.ShipCountry, 
                                       TotalOrderAmount = source.TotalOrderAmount, 
                                       EmployeeId = source.EmployeeId
                        WHEN NOT MATCHED THEN
                            INSERT (OrderId, CustomerId, CustomerName, CustomerContact, OrderDate, ShippedDate, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry, TotalOrderAmount, EmployeeId)
                            VALUES (source.OrderId, source.CustomerId, source.CustomerName, source.CustomerContact, source.OrderDate, source.ShippedDate, source.ShipName, source.ShipAddress, source.ShipCity, source.ShipRegion, source.ShipPostalCode, source.ShipCountry, source.TotalOrderAmount, source.EmployeeId);";
                            mergeCommand.Parameters.AddWithValue("@OrderId", order.OrderId);
                            mergeCommand.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                            mergeCommand.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                            mergeCommand.Parameters.AddWithValue("@CustomerContact", order.CustomerContact);
                            mergeCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                            mergeCommand.Parameters.AddWithValue("@ShippedDate", (object)order.ShippedDate ?? DBNull.Value);
                            mergeCommand.Parameters.AddWithValue("@ShipName", order.ShipName);
                            mergeCommand.Parameters.AddWithValue("@ShipAddress", order.ShipAddress);
                            mergeCommand.Parameters.AddWithValue("@ShipCity", order.ShipCity);
                            mergeCommand.Parameters.AddWithValue("@ShipRegion", order.ShipRegion);
                            mergeCommand.Parameters.AddWithValue("@ShipPostalCode", order.ShipPostalCode);
                            mergeCommand.Parameters.AddWithValue("@ShipCountry", order.ShipCountry);
                            mergeCommand.Parameters.AddWithValue("@TotalOrderAmount", order.TotalOrderAmount);
                            mergeCommand.Parameters.AddWithValue("@EmployeeId", order.EmployeeId);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadTransformedOrdersAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadTransformedOrdersAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }

        // Load Transformed Order Details
        public async Task<bool> LoadTransformedOrderDetailsAsync(IEnumerable<TransformedOrderDetail> orderDetails)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var orderDetail in orderDetails)
                        {
                            var mergeCommand = connection.CreateCommand();
                            mergeCommand.Transaction = transaction;
                            mergeCommand.CommandText = @"
                        MERGE INTO TransformedOrderDetail AS target
                        USING 
                        (
                            VALUES (@OrderDetailId, @ProductId, @Quantity, @UnitPrice, @Discount, @TotalAmount)
                        ) AS source (OrderDetailId, ProductId, Quantity, UnitPrice, Discount, TotalAmount)
                        ON target.OrderDetailId = source.OrderDetailId
                        WHEN MATCHED THEN
                            UPDATE SET ProductId = source.ProductId, 
                                       Quantity = source.Quantity, 
                                       UnitPrice = source.UnitPrice, 
                                       Discount = source.Discount, 
                                       TotalAmount = source.TotalAmount
                        WHEN NOT MATCHED THEN
                            INSERT (OrderDetailId, ProductId, Quantity, UnitPrice, Discount, TotalAmount)
                            VALUES (source.OrderDetailId, source.ProductId, source.Quantity, source.UnitPrice, source.Discount, source.TotalAmount);";
                            mergeCommand.Parameters.AddWithValue("@OrderDetailId", orderDetail.OrderDetailId);
                            mergeCommand.Parameters.AddWithValue("@ProductId", orderDetail.ProductId);
                            mergeCommand.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                            mergeCommand.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);
                            mergeCommand.Parameters.AddWithValue("@Discount", orderDetail.Discount);
                            mergeCommand.Parameters.AddWithValue("@TotalAmount", orderDetail.TotalAmount);
                            await mergeCommand.ExecuteNonQueryAsync();
                        }
                        transaction.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"SqlException in {nameof(LoadTransformedOrderDetailsAsync)}: {sqlEx.Message}");
                        foreach (SqlError error in sqlEx.Errors)
                        {
                            Debug.WriteLine($"Error: {error.Number}, {error.Message}, {error.Procedure}");
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Debug.WriteLine($"General Exception in {nameof(LoadTransformedOrderDetailsAsync)}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

