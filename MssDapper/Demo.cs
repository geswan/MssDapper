﻿using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Resources;

namespace MssDapper;

public class Demo
{
    private TransactionExample _transactionExample;
    private Examples _examples;
    private ResourceManager _rm;
    public Demo(Examples examples,TransactionExample transactionExample, ILogger<Demo> logger)
    {
        _transactionExample=transactionExample;
        _examples = examples;
       _rm = new ResourceManager("MssDapper.Properties.Resources", Assembly.GetExecutingAssembly());
        logger.LogInformation("Demo loaded");
    }
    public async Task Run()
    {

        bool isChooseDemo = true;
        while (isChooseDemo)
        {
            isChooseDemo = await SelectAndRunDemoFromMenu();
        }
    }

    private async Task<bool> SelectAndRunDemoFromMenu()
    {
        Console.Clear();
        Console.WriteLine("Please select a demonstration");
        Console.WriteLine(_rm.GetString("item1"));
        Console.WriteLine(_rm.GetString("item2"));
        Console.WriteLine(_rm.GetString("item3"));
        Console.WriteLine(_rm.GetString("item4"));
        Console.WriteLine(_rm.GetString("item5"));
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(_rm.GetString("item6"));
        Console.WriteLine(_rm.GetString("item7"));
        Console.WriteLine(_rm.GetString("item8"));
        Console.ResetColor();
        Console.WriteLine("9. Exit");
        Console.Write("\r\nSelect an option:1-9 ");
        string? choice = Console.ReadLine();
        Console.Clear();
        choice??=string.Empty;
        return await RunDemo(choice);

    }
    private async Task<bool> RunDemo(string choice)
    {
        return choice switch
        {
            "1" => await _examples.SubtotalGroupByAsync(),
            "2" => await _examples.GroupByACollectionOfParamsAsync(),
            "3" => await _examples.QueryReturningDynamicType(),
            "4" => await _examples.Map2TablesTo1OrdersAAsync(),
            "5" => await _examples.StoredProcedureCustomerOrderHistoryAsync(),
            "6" => await _examples.InsertEmployeeInstanceAAsync(),
            "7" => await _transactionExample.TransactionAsync(),
            "8" => await _examples.BulkInsertAsyncMap(),
            "9" => false,
            _ => true,
        }; 
    }
    
}
