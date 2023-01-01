namespace MssDapper;

public class StoredProcedureId

{      // avoid using the prefix dbo.sp_ when naming procedures
       // as it's used for naming system stored procedures
    public  string CustomerOrderHistory { get; } = "CustOrderHist";
    public string SpInsertEmployee { get; } = "SaveEmployee";
   
}