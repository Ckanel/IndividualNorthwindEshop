using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IndividualNorthwindEshop.Services;
using ETL.Transform.TransformedModels;
using System.Collections.Generic;
using IndividualNorthwindEshop.Models;
using ETL.Transform.DataAccess;
using Newtonsoft.Json;
namespace IndividualNorthwindEshop.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ETLDataExportService _etlDataExportService;

        public DashboardController(ETLDataExportService etlDataExportService)
        {
            _etlDataExportService = etlDataExportService;
        }


        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                SalesData = await _etlDataExportService.GetSalesDataAsync(),
                ProductPerformance = await _etlDataExportService.GetProductPerformancesAsync(),
                CustomerInsights = await _etlDataExportService.GetCustomerInsightsAsync(),
                DiscountEffectiveness = await _etlDataExportService.GetDiscountEffectivenessAsync(),
                SalesForecast = await _etlDataExportService.GetSalesForecastsAsync(),
                EmployeePerformance = await _etlDataExportService.GetEmployeePerformancesAsync()
            };

            ViewBag.SalesDataJson = JsonConvert.SerializeObject(model.SalesData);
            ViewBag.ProductPerformanceJson = JsonConvert.SerializeObject(model.ProductPerformance);
            ViewBag.CustomerInsightsJson = JsonConvert.SerializeObject(model.CustomerInsights);
            ViewBag.SalesForecastJson = JsonConvert.SerializeObject(model.SalesForecast);
            ViewBag.DiscountEffectivenessJson = JsonConvert.SerializeObject(model.DiscountEffectiveness);
            ViewBag.EmployeePerformanceJson = JsonConvert.SerializeObject(model.EmployeePerformance);

            return View(model);
        }


    }
}