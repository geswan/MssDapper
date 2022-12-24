using DataAccess;

namespace MssDapper
{
    public class TransactionExample
    {
        private IDataAccess _dba;
        private readonly SpExampleIds _spIds;
        private Helper _helper;

        public TransactionExample(IDataAccess dba,SpExampleIds spIds, Helper helper)
        {
            _dba = dba;
            _spIds = spIds;
            _helper = helper;
      
        }
        #region Example 7 Transaction Example.
        public async Task<bool> TransactionAsync()
        {

            Employee employee = new()
            {
                FirstName = "Micro",
                LastName = "Mouse",
                BirthDate = new DateTime(2021, 12, 01)
            };
            string sql = _dba.IsSqlServer ? _spIds.InsertEmployeeTSQL: _spIds.InsertEmployeeMySQL;

            var idMicroA = await _dba.QuerySingleAsync<int>(sql, employee);
            var idMicroB = await _dba.QuerySingleAsync<int>(sql, employee);
            Console.WriteLine($"2 Micro mice inserted into the Employees table. Id mouse A is {idMicroA} Id mouse B is {idMicroB}");
            Console.WriteLine($"Executing a transaction to remove both mice...");
            Console.WriteLine($"The Id for Mouse A is correctly set but the Id for Mouse B is set to 0 and should throw an exception");
            await ExecuteTestTransactionAsync(idMicroA, 0);
            (Employee microMouseA, Employee microMouseB) = await FindTransactedMiceAsync(idMicroA, idMicroB);
            Console.WriteLine($"The following Micro mice were found : {microMouseA} {microMouseB}");
            Console.WriteLine($"Repeating the transaction using the correct Ids. Both mice should have been removed.");
            await ExecuteTestTransactionAsync(idMicroA, idMicroB);
            (microMouseA, microMouseB) = await FindTransactedMiceAsync(idMicroA, idMicroB);
            Console.WriteLine($"The following Micro mice were found : {microMouseA} {microMouseB}");
            Console.WriteLine("\r\nPlease press return to continue");
            Console.ReadLine();
            return true;
        }

       

        private async Task ExecuteTestTransactionAsync(int idMicroA, int idMicroB)
        {
            string sqlDelete = @"delete
                           from Employees
                           where EmployeeID=@Id;";
            try
            {
                await _dba.ExecuteTransactionAsync(sqlDelete, new { Id = idMicroA }, sqlDelete, new { Id = idMicroB });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The transaction failed. {ex.Message}");
                Console.ResetColor();
            }

        }
        private async Task<(Employee employeeA, Employee employeeB)> FindTransactedMiceAsync(int idMicroA, int idMicroB)
        {
            Console.WriteLine("Searching the data table for both Micro mice...");
            var microMouseA = await _helper.GetFirstOrDefaultEmployeeAsync(idMicroA);
            var microMouseB = await _helper.GetFirstOrDefaultEmployeeAsync(idMicroB);
            return (microMouseA, microMouseB);
        }
        #endregion

    }
}
