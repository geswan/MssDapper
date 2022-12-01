namespace MssDapper
{
    public class Summary
    {
public int OrderID { get; set; }
 public float Subtotal { get; set; }
       
    public override string ToString()
        {
     return    string.Format("    {0,-10:G}{1,12:C2}",OrderID, Subtotal);
        }
    }
}
