# Project Name : Still a work in progress

## Installation Instructions

1. **Install the Northwind database on your SQL Server.**
    - You can download the Northwind database from here https://github.com/microsoft/sql-server-samples/blob/master/samples/databases/northwind-pubs/instnwnd.sql
    
2. **Clone the Git repository.**
    - Open a terminal window.
    - Navigate to the directory where you want to clone the repository.
    - Run the following command: `git clone https://github.com/Ckanel/IndividualNorthwindEshop.git` 

3. **Update the connection string in the application.**
    - Open the application in Visual Studio.
    - Find the connection string in the `appsettings.json` file 
    - Update the connection string to point to your SQL Server where the Northwind database is installed.

4. **Run the migrations.**
    - Open the Package Manager Console in Visual Studio (`Tools > NuGet Package Manager > Package Manager Console`).
    - Run the following command: `Update-Database`.

After following these steps, you should be able to run the application with the intended database structure .
