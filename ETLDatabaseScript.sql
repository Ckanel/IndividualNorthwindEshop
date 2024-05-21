USE ETLDatabase;

CREATE TABLE CustomerInsights (
    CustomerId NVARCHAR(50) PRIMARY KEY,
    TotalOrders INT,
    TotalSpent DECIMAL
);

CREATE TABLE DiscountEffectiveness (
    DiscountRate FLOAT PRIMARY KEY,
    TotalSales DECIMAL
);

CREATE TABLE EmployeePerformance (
    EmployeeId INT PRIMARY KEY,
    OrdersHandled INT,
    AverageHandlingTime FLOAT
);

CREATE TABLE ProductPerformance (
    ProductId INT PRIMARY KEY,
    TotalQuantitySold INT,
    TotalAmountSold DECIMAL,
    TotalRevenue DECIMAL
);

CREATE TABLE SalesData (
    Year INT,
    Month INT,
    TotalSales DECIMAL,
    PRIMARY KEY (Year, Month)
);

CREATE TABLE SalesForecast (
    Year INT,
    Month INT,
    PredictedSales DECIMAL,
    PRIMARY KEY (Year, Month)
);

CREATE TABLE TransformedOrderDetail (
    OrderDetailId INT PRIMARY KEY,
    ProductId INT,
    UnitPrice DECIMAL,
    Quantity SMALLINT,
    Discount FLOAT,
    TotalAmount DECIMAL
);

CREATE TABLE TransformedOrder (
    OrderId INT PRIMARY KEY,
    OrderDate DATETIME,
    ShippedDate DATETIME,
    ShipName NVARCHAR(50),
    ShipAddress NVARCHAR(255),
    ShipCity NVARCHAR(50),
    ShipRegion NVARCHAR(50),
    ShipPostalCode NVARCHAR(20),
    ShipCountry NVARCHAR(50),
    CustomerId NVARCHAR(50),
    CustomerName NVARCHAR(100),
    CustomerContact NVARCHAR(100),
    TotalOrderAmount DECIMAL
);

