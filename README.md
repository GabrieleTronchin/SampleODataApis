# OData Implementations

  Status of the project: ***Draft***

Target of this project is try two different way of implement OData in .NET.

  About OData:

> OData(Open Data Protocol) protocol is created to provide us filtering,
> expanding, sorting, paging abilities over URL. 

More info: [odata](https://www.odata.org/)

## OData with Entity Framework 

A classic approch is to combine OData and Entity Framework to allow an automatic data manipulation.

[OdataSample](https://github.com/OData/AspNetCoreOData)
[MediumArticle](https://medium.com/@ibrahimozgon/asp-net-core-odata-query-database-over-url-820624beef92)


## Kata query builder with Dynamic EDF Model

This approch use Kata insted of Entity Framework to generate a valid T-SQL.

    SqlKata Query Builder is a powerful Sql Query Builder written in C#.
    It's secure and framework agnostic.
    Inspired by the top Query Builders available, like Laravel Query Builder and Knex.
    
[kata](https://sqlkata.com/)



  
