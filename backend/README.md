# StudentApi (ADO.NET + Stored Procedures)

.NET version: **.NET 8**

This is a sample ASP.NET Core Web API that uses ADO.NET to call SQL Server stored procedures.

## Setup

1. Make sure you have **.NET 8 SDK** and **SQL Server** (or SQL Server Express) installed.
2. Open SQL Server Management Studio (SSMS) and run the script in `sql/StudentDB_Init.sql` to create the database, table, and stored procedures.
3. Open the folder in Visual Studio or VS Code.
4. Restore and run:
   ```
   dotnet restore
   dotnet run
   ```
5. Open Swagger at `https://localhost:5001/swagger` (port may vary).

## Endpoints
- `GET /api/students` - Get all students
- `POST /api/students` - Add a student (body: { name, age, course })
- `PUT /api/students/{id}` - Update a student (body: { name, age, course })
- `DELETE /api/students/{id}` - Delete a student

