using DataAccess;

namespace MssDapper
{
    public class Helper
    {
        private IDataAccess _dba;
//        private List<string> formatstrings = new()
//{
//"    {0,-20:G}   {1,6:G}",
//"    {0,-15:G} {1,-15:G} {2,12:G}",
//"    {0,-20:G} {1,10:G}",
//"    {0,-31:G} {1,10:G} ",
//"    {0,-30:G}{1,12:C2}"
//};
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
    }
}
