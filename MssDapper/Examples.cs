using Dapper;
using DataAccess;
using System.Data;

namespace MssDapper;

public class Examples
{
    private readonly IDatabaseContext _dbContext;
    private readonly StoredProcedureId _storedProcedureId;
    private readonly Helper _helper;
    private readonly int recordsToInsert = 5;

    public Examples(IDatabaseContext dbContext, StoredProcedureId storedProcedureId, Helper helper)
    {
        _dbContext = dbContext;
        _storedProcedureId = storedProcedureId;
        _helper = helper;
    }
    #region Example  Group By SubTotal
    public async Task<bool> SubtotalGroupByAsync()
    {
        string mySql = @"SELECT `Order Details`.OrderID, Sum(`Order Details`.UnitPrice*Quantity*(1-Discount)) AS Subtotal
        FROM `Order Details`
        GROUP BY `Order Details`.OrderID;";

        string tSql = @"SELECT [Order Details].OrderID,Sum([Order Details].UnitPrice*Quantity*(1-Discount)) AS Subtotal
        FROM [Order Details]
        GROUP BY [Order Details].OrderID;";
        string sql = _dbContext.IsSqlServer ? tSql : mySql;
        var summaries = await _dbContext.QueryAsync<Summary>(sql);
        Console.WriteLine(_helper.Format2ColsWide, "OrderID", "Subtotal");
        int count = 0;
        foreach (var summary in summaries)
        {
            Console.WriteLine(_helper.Format2ColsWide, summary.OrderID, summary.Subtotal);
            count++;
        }
        Console.WriteLine(_helper.Format2ColsWide, "OrderID", "Subtotal");
        return _helper.PressReturnToContinue(count);

    }
    #endregion

    #region Example Group By Using a collection of input parameters
    public async Task<bool> GroupByACollectionOfParamsAsync()
    {
        string[] countryArray = new[] { "France", "Germany", "UK", "USA" };
        string sql = @"Select Suppliers.Country As SuppliersCountry,COUNT(*) as ProductCount
             From Suppliers join Products on Suppliers.SupplierID=Products.SupplierID
             where Suppliers.Country in @countries
             Group by Suppliers.Country
             Order by ProductCount Desc;";
        var results = await _dbContext.QueryAsync<(string SuppliersCountry, int ProductCount)>(sql, new { @countries = countryArray });

        Console.WriteLine(_helper.Format2ColsNarrow, "Suppliers' Country", "Product Count");
        int count = 0;
        foreach (var (SuppliersCountry, ProductCount) in results)
        {
            count++;
            Console.WriteLine(_helper.Format2ColsNarrow, SuppliersCountry, ProductCount);
        }
        Console.WriteLine(_helper.Format2ColsNarrow, "Suppliers' Country", "Product Count");
        return _helper.PressReturnToContinue(count);
    }
    #endregion

    #region Example Query that returns a DynamicType
    public async Task<bool> QueryReturningDynamicType()
    {
        var sql = "select City, Region,ContactName from suppliers where Country='USA'";
        dynamic suppliers = await _dbContext.QueryAsyncDynamic(sql);
        Console.WriteLine(_helper.Format3ColsWide, "City", "Region", "ContactName");
        foreach (var supplier in suppliers)
        {
            Console.WriteLine(_helper.Format3ColsWide, supplier.City, supplier.Region, supplier.ContactName);
        }
        Console.WriteLine(_helper.Format3ColsWide, "City", "Region", "Contact Name");
        return _helper.PressReturnToContinue();
    }
    #endregion

    #region Example Map from tableA & tableB to ClassA containing a reference to ClassB
    public async Task<bool> Map2TablesTo1OrdersAAsync()
    {
        string sql = @"select ot.*, et.*
                     from Orders ot
                     inner join Employees et
                     on ot.EmployeeID  = et.EmployeeID
                     order by et.LastName, et.FirstName";
        var employeeOrdersA = await _dbContext.QueryAsync<Order, Employee>(sql,
             (order, employee) =>
             {
                 order.Employee = employee;
                 return order;
             }, splitOn: "EmployeeID");

        Console.WriteLine("Employee Orders ");
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");
        int count = 0;
        foreach (Order emploeeOrderA in employeeOrdersA)
        {
            count++;
            Console.WriteLine(_helper.Format2ColsNarrow, $"{emploeeOrderA?.Employee?.LastName} {emploeeOrderA?.Employee?.FirstName}",
                emploeeOrderA?.OrderDate.ToShortDateString());
        }
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");

        return _helper.PressReturnToContinue(count);
    }
  
    #endregion

    #region Example Map using a Northwind example Stored Procedure
    public async Task<bool> StoredProcedureCustomerOrderHistoryAsync()
    {
        (string paramName, object value) param;
        string customerID = "ANTON";//input param
        param = _dbContext.IsSqlServer ? ("@CustomerID", customerID) : ("AtCustomerID", customerID);
        var paramDic = new Dictionary<string, object>
        {
           { param.paramName, param.value }
        };
        var results = await _dbContext.QueryAsync<(string product, int total)>(_storedProcedureId.CustomerOrderHistory,
        new DynamicParameters(paramDic), commandType: CommandType.StoredProcedure);

        Console.WriteLine(_helper.Format2ColsWide, "Product Name", "Sales");
        int count = 0;
        foreach ((string product, int total) in results)
        {
            count++;
            Console.WriteLine(_helper.Format2ColsWide, product, total);
        }
        return _helper.PressReturnToContinue(count);
    }
    #endregion

    #region Example Insert into Employee table and return the new row identity Id
    public async Task<bool> InsertEmployeeInstanceAAsync()
    {
        Employee employee = new()
        {
            FirstName = "Mini",
            LastName = "Mouse",
            BirthDate = new DateTime(1928, 01, 01)
        };

        string sql = _dbContext.IsSqlServer ? Constants.InsertEmployeeTSQL :Constants.InsertEmployeeMySQL;
        var id = await _dbContext.QuerySingleAsync<int>(sql, employee);
        var foundEmployee = await _helper.GetFirstOrDefaultEmployeeAsync(id);
        Console.WriteLine($"The inserted record is {foundEmployee}");
        Console.WriteLine($"Repeats of this example will insert a cloned mouse with a different ID");
        var clones = await FindEmployeesByNameAsync(employee.LastName, employee.FirstName);
        foreach (Employee clone in clones)
        {
            Console.WriteLine(clone.ToString());
        }
        return _helper.PressReturnToContinue();
    }
    #endregion

    //examples  see also TransactionExample.cs

    #region Example Bulk Insert With Mapping 5 Records

    public async Task<bool> BulkInsertAsyncMap()
    {
        var employees = GenerateRandomEmployeesM(recordsToInsert);
        //Surname needs mapping Surname=>LastName
        Dictionary<string, string> mappingDic = new() { { "Surname", "LastName" } };
        await _dbContext.BulkInsertAsync(employees, mappingDic, table: "Employees");
        Console.WriteLine($"{recordsToInsert} Records Inserted");
        return _helper.PressReturnToContinue();
    }
    #endregion

    #region Other Examples

    public async Task<bool> BulkInsertAsync()
    {
        var employees = GenerateRandomEmployees(recordsToInsert);
        await _dbContext.BulkInsertAsync(employees);
        Console.WriteLine($"{recordsToInsert} Records Inserted");
        return _helper.PressReturnToContinue();
    }
   
   
  
    public async Task<bool> MapToValueTupleAsync()
    {
        string sql = @"SELECT Products.ProductName, Products.UnitPrice
             FROM Products
             order by Products.UnitPrice desc
             LIMIT 10";
        var results = await _dbContext.QueryAsync<(string product, float price)>(sql);
        Console.WriteLine("The 10 Most Expensive Products");
        Console.WriteLine(_helper.Format2ColsWide, "Product", "Price");
        foreach ((string product, float price) in results)
        {
            Console.WriteLine(_helper.Format2ColsWide, product, price);
        }

        return _helper.PressReturnToContinue();
    }

    public async Task<bool> FirstOrDefaultEmployeeAsync()
    {
        string sql = @"select * from Employees where LastName = @LastName and FirstName=@FirstName";
        string lastName = "King";//input param
        string firstName = "Robert";
        var employee = await _dbContext.QueryFirstOrDefaultAsync<Employee>(sql,
        new { LastName = lastName, FirstName = firstName });
        Console.WriteLine($"Query is for {lastName} {firstName}");
        Console.WriteLine($"Found {employee}");
        return _helper.PressReturnToContinue();
    }

    public async Task<bool> InsertEmployeeInstanceAsync()
    {

        Employee employee = new()
        {
            FirstName = "Mickey",
            LastName = "Mouse",
            BirthDate = new DateTime(1928, 01, 01)
        };
        var propValues = new { employee.FirstName, employee.LastName, employee.BirthDate };
        string sql = @"insert into Employees(FirstName,LastName,BirthDate)
                       values (FirstName,LastName,BirthDate)";
        await _dbContext.ExecuteAsync(sql, propValues);
        Console.WriteLine($"{employee} inserted into the Database");
        return _helper.PressReturnToContinue();

    }


  
    #endregion

    #region Private methods
    private async Task<IEnumerable<Employee>> FindEmployeesByNameAsync(string lastName, string firstName)
    {
        string tSql = @"select Top 12 * from Employees where LastName = @LastName and FirstName=@FirstName order by EmployeeID Desc";
        string mySql = @"select * from Employees where LastName = @LastName and FirstName=@FirstName order by EmployeeID Desc limit 12";
        string sql=_dbContext.IsSqlServer ? tSql: mySql;
        var employees = await _dbContext.QueryAsync<Employee>(sql,
        new { LastName = lastName, FirstName = firstName });
        return employees;
    }

    private IEnumerable<Employee> GenerateRandomEmployees(int count)
    {
        foreach (string s in _helper.GenerateRandomStrings(count, 6))
        {
            Employee employee = new()
            {
                EmployeeID = 0,
                LastName = s.ToUpper(),
                FirstName = s,
                BirthDate = DateTime.Today,//constraint is  < getdate()
            };
            yield return employee;
        }
    }

    private IEnumerable<EmployeeM> GenerateRandomEmployeesM(int count)
    {
        foreach (string s in _helper.GenerateRandomStrings(count, 6))
        {
            EmployeeM employee = new()
            {
                EmployeeID = 0,
                Surname = s.ToUpper(),
                FirstName = s,
                BirthDate = DateTime.Today,//constraint <getdate()
            };
            yield return employee; ;
        }
    }
    #endregion
  

}
