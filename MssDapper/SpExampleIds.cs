namespace MssDapper;

public class SpExampleIds

{ // avoid dbo.sp_  as it's used for naming system stored procedures
    public  string CustomerOrderHistory { get; } = "CustOrderHist";
    public string SpInsertEmployee { get; } = "SaveEmployee";
    public string InsertEmployeeSQL { get; } = @"insert into Employees(LastName,FirstName,BirthDate)
                       values(@LastName,@FirstName,@BirthDate);
                       SELECT LAST_INSERT_ID();";

}