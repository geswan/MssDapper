namespace MssDapper;

public class Summary
{
    public int OrderID { get; set; }
    public float Subtotal { get; set; }

    public override string ToString()
    {
        return string.Format("    {0,-10:G}{1,12:C2}", OrderID, Subtotal);
    }
}


public class Order
{
    public int OrderID { get; set; }
    public string? CustomerID { get; set; }//gotcha, assumed it was an int
    public int EmployeeID { get; set; }
    public DateTime OrderDate { get; set; }
    public int ShipperID { get; set; }
    public Employee? Employee { get; set; }
}

public class Employee
{
    public int EmployeeID { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public DateTime BirthDate { get; set; }

    public override string ToString()
    {
        return $" {FirstName} {LastName} {BirthDate.ToShortDateString()} Id {EmployeeID}";
    }
    public  dynamic ToAnonType()
    {
        return new {LastName,FirstName,BirthDate};
    }
}

//used to demo mapping Surname to LastName
public class EmployeeM
{
    public int EmployeeID { get; set; }
    public string? Surname { get; set; }
    public string? FirstName { get; set; }
    public DateTime BirthDate { get; set; }

    public override string ToString()
    {
        return $" {FirstName} {Surname} {BirthDate.ToShortDateString()} Id {EmployeeID}";
    }
    public dynamic ToAnonType()
    {
        return new { Surname, FirstName, BirthDate };
    }
}
public class ServerOptions
{
    public const string Servers = "Servers";

    public string SqlServer { get; set; } = string.Empty;
    public string MySqlServer { get; set; } = string.Empty;
    public string MariaDbServer { get; set; } = string.Empty;
}

//public class DbConnectionOptions
//{
//    public const string DbConnections = "ConnectionStrings";

//    public string MsSql { get; set; } = string.Empty;
//    public string MySql { get; set; } = string.Empty;
//    public string MariaDb { get; set; } = string.Empty;
//}



