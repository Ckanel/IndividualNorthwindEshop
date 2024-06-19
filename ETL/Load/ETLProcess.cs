using System.Threading.Tasks;
using ETL.Extract;
using ETL.Transform;
using ETL.Transform.TransformedModels;
using System.Linq;
using System;
using System.Diagnostics;

namespace ETL.Load
{
    public class ETLProcess
    {
        private readonly OrderTransform _transform;
        private readonly LoadProcess _load;
        private readonly OrderRepository _repository;

        public ETLProcess(OrderTransform transform, LoadProcess load, OrderRepository repository)
        {
            _transform = transform;
            _load = load;
            _repository = repository;
        }

        public async Task RunETLAsync()
        {
            try
            {
                // Step 1: Extract data
                var orders = await _repository.GetAllOrdersWithDetailsAsync();
                var employee = await _repository.GetEmployeeAsyncDetails();
                var orderDetails = orders.SelectMany(order => order.OrderDetails);
                var product = await _repository.GetProductsWithDetailsAsync();

                // Step 2: Transform data
                var transformedOrders = _transform.TransformOrders(orders);
                var transformedOrderDetails = _transform.TransformOrderDetails(orderDetails);

                var insights = _transform.CalculateCustomerInsights(orders);
                var productPerformances = _transform.IdentifyBestSellingProducts(orders, product);

                // Employee performance
                var employeePerformances = _transform.CalculateEmployeePerformance(orders,employee);

                // Discount effectiveness
                var discountEffectiveness = _transform.AnalyzeDiscountEffectiveness(orders);

                // Sales forecast and total sales
                var totalSales = _transform.CalculateTotalSales(orders);
                var salesForecasts = _transform.PredictSales(totalSales);

                // Step 3: Load data

                Debug.WriteLine("Calling LoadCustomerInsightsAsync...");
                var successInsights = await _load.LoadCustomerInsightsAsync(insights);
                Debug.WriteLine($"LoadCustomerInsightsAsync returned: {successInsights}");

                Debug.WriteLine("Calling LoadProductPerformanceAsync...");
                var successPerformance = await _load.LoadProductPerformanceAsync(productPerformances);
                Debug.WriteLine($"LoadProductPerformanceAsync returned: {successPerformance}");

                Debug.WriteLine("Calling LoadEmployeePerformanceAsync...");
                var successEmployeePerformance = await _load.LoadEmployeePerformanceAsync(employeePerformances);
                Debug.WriteLine($"LoadEmployeePerformanceAsync returned: {successEmployeePerformance}");

                Debug.WriteLine("Calling LoadDiscountEffectivenessAsync...");
                var successDiscountEffectiveness = await _load.LoadDiscountEffectivenessAsync(discountEffectiveness);
                Debug.WriteLine($"LoadDiscountEffectivenessAsync returned: {successDiscountEffectiveness}");

                Debug.WriteLine("Calling LoadTotalSalesAsync...");
                var successTotalSales = await _load.LoadTotalSalesAsync(totalSales);
                Debug.WriteLine($"LoadTotalSalesAsync returned: {successTotalSales}");

                Debug.WriteLine("Calling LoadSalesForecastAsync...");
                var successSalesForecast = await _load.LoadSalesForecastAsync(salesForecasts);
                Debug.WriteLine($"LoadSalesForecastAsync returned: {successSalesForecast}");

                Debug.WriteLine("Calling LoadTransformedOrdersAsync...");
                var successTransformOrders = await _load.LoadTransformedOrdersAsync(transformedOrders);
                Debug.WriteLine($"LoadTransformedOrdersAsync returned: {successTransformOrders}");

                Debug.WriteLine("Calling LoadTransformedOrderDetailsAsync...");
                var successTransformOrderDetails = await _load.LoadTransformedOrderDetailsAsync(transformedOrderDetails);
                Debug.WriteLine($"LoadTransformedOrderDetailsAsync returned: {successTransformOrderDetails}");
                // Check all load successes
                if (successInsights &&
            successPerformance &&
            successEmployeePerformance &&
            successDiscountEffectiveness &&
            successTotalSales &&
            successSalesForecast &&
            successTransformOrders &&
            successTransformOrderDetails)
                {
                    Debug.WriteLine("ETL process completed successfully");
                }
                else
                {
                    Debug.WriteLine("ETL process failed");
                    if (!successInsights) Debug.WriteLine("Customer Insights Loading Failed");
                    if (!successPerformance) Debug.WriteLine("Product Performance Loading Failed");
                    if (!successEmployeePerformance) Debug.WriteLine("Employee Performance Loading Failed");
                    if (!successDiscountEffectiveness) Debug.WriteLine("Discount Effectiveness Loading Failed");
                    if (!successTotalSales) Debug.WriteLine("Total Sales Loading Failed");
                    if (!successSalesForecast) Debug.WriteLine("Sales Forecast Loading Failed");
                    if (!successTransformOrders) Debug.WriteLine("Transformed Orders Loading Failed");
                    if (!successTransformOrderDetails) Debug.WriteLine("Transformed Order Details Loading Failed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ETL process encountered an error: {ex.Message}");
            }
        }
    }
}

