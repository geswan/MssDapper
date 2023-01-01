using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MssDapper
{
    internal static class Constants
    {
        public const string InsertEmployeeMySQL= @"insert into Employees(LastName,FirstName,BirthDate)
                       values(@LastName,@FirstName,@BirthDate);
                       SELECT LAST_INSERT_ID();";
        public const string InsertEmployeeTSQL= @"insert into Employees(LastName,FirstName,BirthDate)
                       values(@LastName,@FirstName,@BirthDate);
                       SELECT @@IDENTITY; ";

       
    }
}
