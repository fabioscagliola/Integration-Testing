# McGarlet Sale Prediction

*An ASP.NET Core web API that uses machine learning to predict McGarlet's sales*

## Introduction

The solution consists of two projects: **McGarletSalePrediction** and **McGarletSalePredictionTest**.

 - **McGarletSalePrediction** is an ASP.NET Core web API that uses EF Core and Migrations with Microsoft SQL Server, and machine learning to predict McGarlet's sales.
 - **McGarletSalePredictionTest** is an NUnit test project responsible for the integration testing of the web API and for mocking the database context in order to use an SQLite in-memory database.

## Remarks

The machine learning model is not yet properly trained.

The database is not used: forecasted sales are returned by a web API method.

## Setup

Due to hardcoded paths, temporarily take the following steps in order to setup the development environment:

 - Clone the repository to `C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction`
 - Extract the CSV file from `C:\Data\thesoftwaretailors\McGarlet-Sale-Prediction.NET\Data\Data.zip`

