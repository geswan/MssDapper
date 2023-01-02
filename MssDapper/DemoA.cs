

namespace MssDapper;


public class DemoA
{
    private readonly TransactionExample _transactionExample;
    private readonly Examples _examples;
    private readonly List<MenuItem> _menuItems;
    public DemoA(Examples examples, TransactionExample transactionExample)
    {
        _transactionExample = transactionExample;
        _examples = examples;
        _menuItems = new()
        {
             new MenuItem("Group By SubTotal.", _examples.SubtotalGroupByAsync),
             new MenuItem("Group By Using a collection of input parameters", _examples.GroupByACollectionOfParamsAsync),
             new MenuItem("Query that returns a Dynamic Type",_examples.QueryReturningDynamicType),
             new MenuItem("Map from tableA & tableB to ClassA containing a reference to ClassB" ,_examples.Map2TablesTo1OrdersAAsync),
             new MenuItem("Map using a Northwind example Stored Procedure",_examples.StoredProcedureCustomerOrderHistoryAsync),
             new MenuItem("Insert into Employee table and return the new row identity Id",_examples.InsertEmployeeInstanceAAsync,isInsertExample:true),
             new MenuItem("Transaction Example.",_transactionExample.TransactionAsync,isInsertExample:true),
             new MenuItem("Bulk Insert With Mapping (5 Records)", _examples.BulkInsertAsyncMap,isInsertExample:true),
             new MenuItem("Exit",async()=>await Task.FromResult(false))
        };
        int i = 1;
        //reIndex the list.It facilitates editing the example menu items.
        foreach (var item in _menuItems)
        {
            item.Index = i;
            i++;
        }

    }
    public async Task Run()
    {

        bool isContinue = true;
        while (isContinue)
        {
            isContinue = await SelectAndRunDemoFromMenu();
        }
    }
    private async Task<bool> SelectAndRunDemoFromMenu()
    {
        Console.Clear();
        Console.WriteLine("Please select a demonstration");
        foreach (var item in _menuItems)
        {
            item.WriteToConsole();
        }
        Console.Write($"\r\n Select an option:1-{_menuItems.Count} ");
        string? choice = Console.ReadLine();
        Console.Clear();
        if (!int.TryParse(choice, out int index))
        {
           
            return true;//isContinue
        }
        var menuItem = _menuItems.FirstOrDefault((item) => item.Index == index);
        if (menuItem == null) return true; 
        return await menuItem.Example!();
    }
}
