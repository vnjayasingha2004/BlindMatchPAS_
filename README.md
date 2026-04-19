Install these first:

.NET SDK
SQL Server or SQL Server LocalDB
Visual Studio 2022 or newer

RUN -  dotnet tool install --global dotnet-ef


For Database :

Run-  
dotnet ef database update --project src/BlindMatchPAS.Web --startup-project src/BlindMatchPAS.Web



---

## Also create this file
Create:

`src/BlindMatchPAS.Web/appsettings.example.json`

Paste this:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": yor db connection string
  }
}