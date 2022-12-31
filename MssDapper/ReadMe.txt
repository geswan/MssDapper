  
  RUNNING THE APPLICATION
  To enable the app to run it is necessary to edit the connection strings in appsettings.json . 
  The MySql string should have a 'password=mypassword' statement added to it
  My preference is to make use of 'User Secrets' to avoid having to reveal the password.
  They are easy to configure and are effective in removing passwords
  from the solution
   1. Right click on MssDapper project and select 'Manage User Secrets'
   2. Copy and paste the ConnectionStrings key/value pair into secrets.json.
   3. Add your password to the MySql connection in the secrets.json file only. 
      It should look like this
   {
  "ConnectionStrings": {
    "MsSql": "Data Source=(localdb)\\ProjectModels;Initial Catalog=Northwind;Integrated Security=True",
    "MySql": "Server=127.0.0.1;user ID=root;Password=yourPassword; Database=northwind"
    }
   }
  4. Save secrets.json and add to the Configuration region in Program.cs
  'builder.Configuration.AddUserSecrets<Program>();'
  
  Configuration will look in secrets.json for the required connection strings before looking
  in appsettings. If they are found, the connection strings in appsettings will not be used
  
  To run with MySql or MariaDb databases, in Program.cs 'Add services' region replace
  services.AddScoped<IConnectionCreator, MsSqlConnectionCreator>()
  with:
   services.AddScoped<IConnectionCreator, MySqlConnectionCreator>()

  INSTALLING THE NORTHWIND DATABASE
  The Sql Create Database statements differ between TSql and MySql so you need to get 
  the right version
  MySql Link: Zip file
  https://www.aspsnippets.com/Handlers/DownloadFile.ashx?File=9cb579c6-86db-4596-84c3-d549428fdcf5.zip
  In my version, the 'Notes' field in the 'Employees' table had the constraint 'Not null', that needs 
  to be edited and set to Null. To install, run the script in MySQL Workbench. If using MariaDb, use
  HeidiSQL
  
  TSql Microsoft Sql Server.  Copy and paste from
  https://github.com/microsoft/sql-server-samples/raw/master/samples/databases/northwind-pubs/instnwnd.sql
  To install, run the script in Microsoft Sql Server Management Studio
