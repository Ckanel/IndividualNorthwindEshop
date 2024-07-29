# Integrating Extract-Transform-Load (ETL) process in an E-Commerce Web Application.
 This was web application was developed for my dissertation project. It includes an E-Commerce web application, along with a custom made ETL process. The integration of the ETL process in 
the E-Commerce web application, was done as a service and the visualization of the transformed data by using Chart.JS. Finally, ETL Unit tests were also created to test the ETL process before its integration. The 
approach used was Database First, using a clone of the Microsoft Northwind Database for the E -
Commerce structure. 

## Installation 

1. Restore the NuGet Packages.
2. Create an Initial Migration.
3. Update the Database.
4. Fix the connection strings and the configuration to your desire
5. Run the ETL database script on a new database called ETLDatabase or create a migration for it.
6. Set up the dependencies of the main project. Install AspNetCore Identity and EF Core, along with X.PagedList. Make sure the dependencies between the commondata library and the ETL.csproj of the projecj are in place.
7. Set up the dependencies of the testing project. Install EFCore, .Net test sdk, Moq ( for mock data testing) and xunit. Make sure to configurate the ETL process dependencies. 
8. Set up the dependencies of the ETL process. Instal EFCore, EFCore Design, SqlClient and AspNetCore components. Also make sure to configure the commondata library dependency.
9. Set up CommonData lib dependencies by installing EFCore, AspNetCore Identity, EFcore Tools,Sql Server.
10. After ensuring all the dependencies are in place, run the application.

## Technological stack
### Frameworks & Libraries
* .NET 8: The core framework used for building the application.
* ASP.NET Core MVC: Utilized for creating the MVC architecture and handling HTTP requests and responses.
### Data Access
* Entity Framework Core (EF Core): Used as the Object-Relational Mapper (ORM) for data extraction and manipulation.
* ADO.NET: Employed for the data loading process to ensure efficient data operations.
### User Management
* ASP.NET Identity: Used for managing user authentication and authorization, ensuring secure access control.
### Concurrency
* Optimistic Concurrency: Implemented to handle concurrency issues while processing orders, preventing conflicts during simultaneous transactions.
### Frontend
* Tailwind CSS: A utility-first CSS framework used for designing responsive and modern user interfaces.
### Email Service
* Mailgun: Integrated as the email delivery service to handle all email communications for the e-commerce platform.
### Testing
* xUnit: Used for testing purposes to ensure the reliability and stability of the ETL pipeline.
## Prerequisites 
* .NET 8 SDK
* SQL Server
* I would recommend using visual studio to run this project, as you would need to set up the dependencies. 
## Abstract
This dissertation serves as research on the Extract-Transform-Load (ETL) process and 
modern software design principles. It will expand on the development of an e-commerce web 
application that is integrated with a custom ETL process. It will try to explain what the ETL 
process is, what e-commerce is, and why they are used in conjunction in the modern software 
development world. It will try to justify the technologies that were used during the production 
and explain why a Northwind database clone was initially needed for the development. The 
changes made to the Northwind will be thoroughly documented, and an analysis of the 
reasons behind them will be provided, along with the new data warehouse that will be created 
for the ETL. A thorough look into the architecture of the system, in addition to the design 
patterns used, will also be stated. Finally, it will present the implementation process of such 
an application while acknowledging the system limitations and the need for future 
improvements
