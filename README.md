# ASP.NET Core Integration Testing

The purpose of this solution is to implement integration testing of an ASP.NET Core web API that uses EF Core and Migrations with Microsoft SQL Server, mocking the database context in order to use SQLite.

The solution consists of two projects: **WebApi** and **WebApiTest**:

 - **WebApi** is the ASP.NET Core web API that uses EF Core and Migrations with Microsoft SQL Server.
 - **WebApiTest** is the NUnit test project responsible for the integration testing of the web API and mocking the database context in order to use SQLite.

## WebApi

**WebApi** is the ASP.NET Core web API that uses EF Core and Migrations with Microsoft SQL Server.

It references the following NuGet packages:

 - Microsoft.AspNetCore.OpenApi
 - Microsoft.EntityFrameworkCore.SqlServer
 - Microsoft.EntityFrameworkCore.Tools
 - Swashbuckle.AspNetCore
 - Swashbuckle.AspNetCore.Annotations

It includes a database context called **WebApiDbContext** containing one entity only, **Person**, including the **Id**, **FName**, and **LName** attributes/fields/properties, whatever you prefer calling them.

The Migration that takes care of the database in Microsoft SQL Server was created and executed by issuing the following commands at the Package Manager Console:

```
Install-Package Microsoft.EntityFrameworkCore.Tools
Add-Migration Migration001
Update-Database
```

I removed the timestamp from the beginning of the name of the file. Migrations are executed in alphabetical order anyway, and I prefer using a suffix rather than a prefix.

The one and only controller called **Person** allows to create, read, update, and delete people records.

## WebApiTest

**WebApiTest** is the NUnit test project responsible for the integration testing of the web API and mocking the database context in order to use SQLite.

It references the following NuGet packages:

 - Microsoft.AspNetCore.Mvc.Testing
 - Microsoft.EntityFrameworkCore.Design
 - Microsoft.EntityFrameworkCore.Sqlite
 - Microsoft.NET.Test.Sdk
 - NUnit
 - NUnit3TestAdapter
 - NUnit.Analyzers
 - coverlet.collector

It also references the **WebApi** project, of course.

Integration tests are executed by means of an instance of the **WebApiTestWebApplicationFactory** class, which replaces the Microsoft SQL Server with SQLite at run-time.

The **DesignTimeDbContextFactory** class is responsible for replacing the database at design-time as well, in order to be able to create and execute the Migration that takes care of the database in SQLite from the Package Manager Console, using the following commands:

```
Install-Package Microsoft.EntityFrameworkCore.Tools
Add-Migration Migration001 -Project WebApiTest
Update-Database
```

The tests included in the **PersonControllerTest** class, based on the **BaseTest** class (where the **WebApiTestWebApplicationFactory** class is instantiated and disposed of), ensure that the **Person** controller allows to create, read, update, and delete people records.

## Setup

The following steps must be completed before running the ASP.NET Core web API or the integration tests:

 1. Create a user on Microsoft SQL Server named "WebApi" using the "XXXX" password and assign the **dbcreator** server-level role to the user.
 1. Create the `C:\TEMP\WebApi` folder on the filesystem where the SQLite database will be stored.

The above username, password, and path (as well as the name or IP address of the Microsoft SQL Server computer and additional settings) can be customized by editing the `appsettings.json` files of the two projects.

