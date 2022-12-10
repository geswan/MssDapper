using DataAccess;
using System;

namespace MssDapper
{
    public class Helper
    {
        private IDataAccess _dba;
        private Random random = new Random();
        public Helper(IDataAccess dba)
        {
            _dba = dba;
        }

        public string Format2ColsNarrow { get; } = "    {0,-20:G} {1,10:G}";
        public string Format2ColsWide { get; } = "    {0,-30:G}{1,12:C2}";

        public string Format3ColsWide { get; } = "    {0,-14:G} {1,-6:G}    {2,-25:G}";


        public bool PressReturnToContinue(int count = -1)
        {
            string msg = $"\r\nPlease press return to continue";
            msg = count == -1 ? msg : $"\r\nFound {count} records {msg}";

            Console.WriteLine(msg);
            Console.ReadLine();

            return true;
        }

        public async Task<Employee> GetFirstOrDefaultEmployeeAsync(int id)
        {
            string sql = @"select * from Employees where EmployeeID = @Id";
            Employee employee = await _dba.QueryFirstOrDefaultAsync<Employee>(sql,
            new { Id = id });
            return employee;
        }

        public IEnumerable<string> GenerateRandomStrings(int count, int length)
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
}
