# OData Implementations

**Project Status:** *Draft*

The aim of this project is to explore two different approaches to implementing OData in .NET.

### About OData:

The Open Data Protocol (OData) is designed to provide functionalities such as filtering, expanding, sorting, and paging via URLs.

More information: [odata](https://www.odata.org/)

## OData with Entity Framework

One common approach is to integrate OData with Entity Framework, enabling automatic data manipulation.

- Sample Repository: [OdataSample](https://github.com/OData/AspNetCoreOData)
- Relevant Article: [MediumArticle](https://medium.com/@ibrahimozgon/asp-net-core-odata-query-database-over-url-820624beef92)

## Kata Query Builder with Dynamic EDF Model

This approach utilizes Kata instead of Entity Framework to generate valid T-SQL queries.

**SqlKata Query Builder** is a robust SQL query builder implemented in C#. It prioritizes security and is framework-agnostic, drawing inspiration from top Query Builders like Laravel Query Builder and Knex.

[Learn more about Kata](https://sqlkata.com/)



  
