using Dapper;
using DataAccess;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Reflection;

namespace MssDapper;

public class Examples
{
    private IDataAccess _dba;
    private readonly SpExampleIds _spIds;
    private ILogger _logger;
    private Helper _helper;
    private Random random = new Random();

    public Examples(IDataAccess dba, ILoggerFactory loggerFactory, SpExampleIds spIds, Helper helper)
    {
        _logger = loggerFactory.CreateLogger<Examples>();
        _dba = dba;
        _spIds = spIds;
        _helper = helper;

    }
    public async Task<bool> SubtotalGroupByAsync()
    {

        string sql = @"SELECT `Order Details`.OrderID, 
        Sum((`Order Details`.UnitPrice*Quantity*(1-Discount)/100)*100) AS Subtotal
        FROM `Order Details`
        GROUP BY `Order Details`.OrderID;";
        var summaries = await _dba.QueryAsync<Summary>(sql, null);
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
  // change this example
    public async Task<bool> QueryReturningDynamicType()
    {
        var sql = "select City, Region,ContactName from suppliers where Country='USA'";
        dynamic suppliers = await _dba.QueryAsyncDynamic(sql, null);
        Console.WriteLine(_helper.Format3ColsWide, "City", "Region", "ContactName");
        foreach (var supplier in suppliers)//fastest way
        {
            Console.WriteLine(_helper.Format3ColsWide,supplier.City,supplier.Region,supplier.ContactName);
        }
        Console.WriteLine(_helper.Format3ColsWide, "City", "Region", "Contact Name");
        return _helper.PressReturnToContinue();
    }
    //https://www.codeproject.com/Articles/5275840/Dynamic-Query-Builder-for-Dapper
    public async Task BulkInsertEmployeesAsync(IEnumerable<Employee> employees)
    {
        //build a list of the Employees table column names 
        //to indicate where the values are to be inserted

        List<string> colNames = new();
        Employee e;

        colNames.Add(nameof(e.LastName));
        colNames.Add(nameof(e.FirstName));
        colNames.Add(nameof(e.BirthDate));


        //insert the input parameters into a key/value dictionary
        //the parameters are designated @p0 to @pn
        Dictionary<string, object> paramDic = new();

        int i = -1;
        foreach (var employee in employees)
        {

            paramDic.Add($"@p{++i}", employee.LastName);
            paramDic.Add($"@p{++i}", employee.FirstName);
            paramDic.Add($"@p{++i}", employee.BirthDate);


        }

        //pass the required parameters to the BulkInsertAsync Method
        await _dba.BulkInsertAsync("Employees", paramDic, colNames);

    }
    //public async Task BulkInsertItemsAsync<T>(
    //    string table,
    //    IEnumerable<T> items,
    //    Dictionary<string,string>? mappingDic =null
    //    )
    //{
    //    //build a list of the table column names 
    //    //to indicate where the values are to be inserted
    //   Type type=typeof(T);
    //    List<string> colNames = new();
    //    //select only public none-static properties
    //    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    //    foreach (var prop in properties)
    //        colNames.Add(prop.Name);
    //    if(mappingDic!=null)
    //    {
    //        for(int j=0;j<colNames.Count;j++)
    //        {
    //            if (mappingDic.ContainsKey(colNames[j]))
    //            {
    //                colNames[j] = mappingDic[colNames[j]];
    //            }
    //        }
    //    }
    //    //insert the input parameters into a key/value dictionary
    //    //the parameters are designated @p0 to @pn
    //    Dictionary<string, object> paramDic = new();

    //    int i = -1;
    //    foreach (var item in items)
    //    {
    //        foreach (var prop in properties)
    //        {
    //            paramDic.Add($"@p{++i}", prop.GetValue(item)!);
    //        }
    //    }

    //    //pass the required parameters to the BulkInsertAsync Method
    //    await _dba.BulkInsertAsync(table, paramDic, colNames);

    //}
    public async Task<bool> BulkInsertAsync()
    {
        int recordsToInsert = 5;
        var employees = GenerateRandomEmployeesM(recordsToInsert);
        Dictionary<string,string>mappingDic= new() { { "Surname", "LastName" } };
     await   _dba.BulkInsertAsync<EmployeeM>("Employees",employees,mappingDic);
        Console.WriteLine($"{recordsToInsert} Records Inserted");
        return _helper.PressReturnToContinue();
    }

    public async Task<bool> GroupByACollectionOfParamsAsync()
    {
        string[] countryArray = new[] { "France", "Germany", "UK", "USA" };
        string sql = @"Select Suppliers.Country As SuppliersCountry,COUNT(*) as ProductCount
             From Suppliers join Products on Suppliers.SupplierID=Products.SupplierID
             where Suppliers.Country in @countries
             Group by Suppliers.Country
             Order by ProductCount Desc;";
        var results = await _dba.QueryAsync<(string SuppliersCountry, int ProductCount)>(sql, new { @countries = countryArray });

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

    public async Task<bool> Map2TablesTo1ClassAAsync()
    {
        string sql = @"select o.EmployeeID,o.OrderId, e.EmployeeID,e.FirstName,e.LastName
                     from Orders o
                     inner join Employees e
                     on o.EmployeeID  = e.EmployeeID
                     order by e.LastName, e.FirstName";
        var employeeOrders = await _dba.QueryAsync<Order, Employee>(sql,
             (order, employee) =>
             {
                 order.Employee = employee;
                 return order;
             }, splitOn: "EmployeeID");

        Console.WriteLine("Employee Orders ");
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");
        int count = 0;
        foreach (var emploeeOrderA in employeeOrders)
        {
            count++;
            Console.WriteLine(_helper.Format2ColsNarrow, $"{emploeeOrderA.Employee.LastName} {emploeeOrderA.Employee.FirstName}",
                emploeeOrderA.OrderDate.ToShortDateString());
        }
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");

        return _helper.PressReturnToContinue(count);
    }


    public async Task<bool> StoredProcedureCustomerOrderHistoryAsync()
    {
        string connectionId = _dba.GetConnectionId();
        (string paramName, object value) param;
        string customerID = "ANTON";//input param
        param = connectionId == "MsSql" ? ("@CustomerID", customerID) : ("AtCustomerID", customerID);
        var paramDic = new Dictionary<string, object>
        {
           { param.paramName, param.value }
        };
        var results = await _dba.QueryAsync<(string product, int total)>(_spIds.CustomerOrderHistory,
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
    public async Task<bool> MapToValueTupleAsync()
    {
        string sql = @"SELECT Products.ProductName, Products.UnitPrice
             FROM Products
             order by Products.UnitPrice desc
             LIMIT 10";
        var results = await _dba.QueryAsync<(string product, float price)>(sql, null);
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
        var employee = await _dba.QueryFirstOrDefaultAsync<Employee>(sql,
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
        await _dba.ExecuteAsync(sql, propValues);
        Console.WriteLine($"{employee} inserted into the Database");
        return _helper.PressReturnToContinue();

    }

    public async Task<bool> InsertEmployeeInstanceAAsync()
    {
        Employee employee = new()
        {
            FirstName = "Mini",
            LastName = "Mouse",
            BirthDate = new DateTime(1928, 01, 01)
        };
        string connectionId = _dba.GetConnectionId();
        string sql = connectionId == "MySql" ? _spIds.InsertEmployeeMySQL : _spIds.InsertEmployeeMssSQL;
        var id = await _dba.QuerySingleAsync<int>(sql, employee);
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

    public async Task<bool> Map2TablesTo1OrdersAAsync()
    {
        string sql = @"select ot.*, et.*
                     from Orders ot
                     inner join Employees et
                     on ot.EmployeeID  = et.EmployeeID
                     order by et.LastName, et.FirstName";
        var employeeOrdersA = await _dba.QueryAsync<Order, Employee>(sql,
             (order, employee) =>
             {
                 order.Employee = employee;
                 return order;
             }, splitOn: "EmployeeID");

        Console.WriteLine("Employee Orders ");
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");
        int count = 0;
        foreach (var emploeeOrderA in employeeOrdersA)
        {
            count++;
            Console.WriteLine(_helper.Format2ColsNarrow, $"{emploeeOrderA.Employee.LastName} {emploeeOrderA.Employee.FirstName}",
                emploeeOrderA.OrderDate.ToShortDateString());
        }
        Console.WriteLine(_helper.Format2ColsNarrow, "Employee Name", "Order Date");

        return _helper.PressReturnToContinue(count);
    }

    private async Task<IEnumerable<Employee>> FindEmployeesByNameAsync(string lastName, string firstName)
    {
        string sql = @"select * from Employees where LastName = @LastName and FirstName=@FirstName order by EmployeeID Desc";
        var employees = await _dba.QueryAsync<Employee>(sql,
        new { LastName = lastName, FirstName = firstName });

        return employees.Take(12);
    }

    private IEnumerable<Employee> GenerateRandomEmployees(int count)
    {
        foreach (string s in GenerateRandomStrings(count, 6))
        {
            Employee employee = new()
            {
                EmployeeID = 0,
                LastName = s.ToUpper(),
                FirstName = s,
                BirthDate = DateTime.Now
            };
            yield return employee; ;
        }
    }

    private IEnumerable<EmployeeM> GenerateRandomEmployeesM(int count)
    {
        foreach (string s in GenerateRandomStrings(count, 6))
        {
            EmployeeM employee = new()
            {
                EmployeeID = 0,
                Surname = s.ToUpper(),
                FirstName = s,
                BirthDate = DateTime.Now
            };
            yield return employee; ;
        }
    }
    private IEnumerable<string> GenerateRandomStrings(int count, int length)
    {
        var startPos = (int)'a';
        for (int n = 0; n < count; n++)
        {
            var array = new char[length];
            for (int i = 0; i < length; i++)
            {
                int r = random.Next(startPos, startPos + 26);
                //a random mix of uppercase and lowercase letters
                array[i] = r % 2 == 0 ? (char)r : char.ToUpper((char)r);
            }
            yield return new string(array);

        }
    }

}
