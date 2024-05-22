USE ETLDatabase;

-- Creating the CustomerInsights table
CREATE TABLE CustomerInsights (
    CustomerId NVARCHAR(50) PRIMARY KEY,
    TotalOrders INT NOT NULL,
    TotalSpent DECIMAL(18, 2) NOT NULL
);

-- Creating the DiscountEffectiveness table
CREATE TABLE DiscountEffectiveness (
    DiscountRate FLOAT PRIMARY KEY,
    TotalSales DECIMAL(18, 2) NOT NULL
);

-- Creating the EmployeePerformance table
CREATE TABLE EmployeePerformance (
    EmployeeId INT PRIMARY KEY,
    OrdersHandled INT NOT NULL,
    AverageHandlingTime FLOAT NOT NULL
);

-- Creating the ProductPerformance table
CREATE TABLE ProductPerformance (
    ProductId INT PRIMARY KEY,
    TotalQuantitySold INT NOT NULL,
    TotalAmountSold DECIMAL(18, 2) NOT NULL,
    TotalRevenue DECIMAL(18, 2) NOT NULL
);

-- Creating the SalesData table
CREATE TABLE SalesData (
    Year INT NOT NULL,
    Month INT NOT NULL,
    TotalSales DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (Year, Month)
);

-- Creating the SalesForecast table
CREATE TABLE SalesForecast (
    Year INT NOT NULL,
    Month INT NOT NULL,
    PredictedSales DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (Year, Month)
);

-- Creating the TransformedOrderDetail table
CREATE TABLE TransformedOrderDetail (
    OrderDetailId INT PRIMARY KEY,
    ProductId INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    Quantity SMALLINT NOT NULL,
    Discount FLOAT NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES ProductPerformance(ProductId) -- assuming ProductId in ProductPerformance is related
);

-- Creating the TransformedOrder table
CREATE TABLE TransformedOrder (
    OrderId INT PRIMARY KEY,
    OrderDate DATETIME NOT NULL,
    ShippedDate DATETIME,
    ShipName NVARCHAR(50) NOT NULL,
    ShipAddress NVARCHAR(255) NOT NULL,
    ShipCity NVARCHAR(50) NOT NULL,
    ShipRegion NVARCHAR(50),
    ShipPostalCode NVARCHAR(20),
    ShipCountry NVARCHAR(50) NOT NULL,
    CustomerId NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    CustomerContact NVARCHAR(100),
    TotalOrderAmount DECIMAL(18, 2) NOT NULL,
    EmployeeId INT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES CustomerInsights(CustomerId),
    FOREIGN KEY (EmployeeId) REFERENCES EmployeePerformance(EmployeeId)
);