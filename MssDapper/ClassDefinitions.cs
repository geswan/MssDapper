namespace MssDapper;
//public class Product
//{
//    public int ProductId { get; set; }
//    public string ProductName { get; set; }
  
//    public Category Category { get; set; }
//}
//public class Category
//{
//    public int CategoryId { get; set; }
//    public string CategoryName { get; set; }
   
//    //public ICollection<Product> Products { get; set; }
//}



public class Order
{
    public int OrderID { get; set; }
    public string CustomerID { get; set; }//gotcha, assumed it was an int
    public int EmployeeID { get; set; }
    public DateTime OrderDate { get; set; }
    public int ShipperID { get; set; }
    public Employee Employee { get; set; }
}

public class Employee
{
    public int EmployeeID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
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


public class EmployeeM
{
    public int EmployeeID { get; set; }
    public string Surname { get; set; }
    public string FirstName { get; set; }
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

//public class EmployeeOrder : Order
//{
//    public string LastName { get; set; }
//    public string FirstName { get; set; }


//}

//public class EmployeeOrderA : Order
//{
//    public Employee Employee { get; set; }
//}
//public class OrderA
//{
//    public int OrderID { get; set; }
//    public string CustomerID { get; set; }//gotcha, assumed it was an int
//    public int EmployeeID { get; set; }
//    public DateTime OrderDate { get; set; }
//    public int ShipperID { get; set; }

//    public Employee Employee { get; set; }
//}


