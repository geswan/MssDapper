using Dapper;
using System.Data;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public static class Extensions
    {
        public async static Task  BulkInsertAsync<T>(
            this IDbConnection connection,
           IEnumerable<T> items,
            Dictionary<string,string> mappingDic,
             string? table=null
            )
        {
            //build a list of the table column names 
            //to indicate where the values are to be inserted
            Type type = typeof(T);
            string? tableName = table ?? type.Name+'s';
            List<string> colNames = new();
            //select only public non-static properties
            //assume first property is identity and skip it
            var properties = (type.GetProperties(BindingFlags.Public | BindingFlags.Instance)).Skip(1);
         
            foreach (var prop in properties)
            {
                //map property names to Column names if needed
                string name = mappingDic != null && mappingDic.ContainsKey(prop.Name) ?
                              mappingDic[prop.Name] : prop.Name;

                colNames.Add(name);
            }

            //insert the input parameters into a key/value dictionary
            //the parameters are designated @p0 to @pn
            Dictionary<string, object> paramDic = new();

            int i = -1;
            foreach (var item in items)
            {
                foreach (var prop in properties)
                {
                    paramDic.Add($"@p{++i}", prop.GetValue(item)!);
                }
            }

            
           string sql= BuildSqlInsert(tableName, paramDic, colNames);
            await connection.ExecuteAsync(sql, new DynamicParameters(paramDic));

        }

        private  static string  BuildSqlInsert(
      string tableName,
      Dictionary<string, object> paramDic,
      IEnumerable<string> colNames
      )
        {
           
            int columnsPerRow = colNames.Count();
            StringBuilder sb = new("(@p0");
            for (int n = 1; n < paramDic.Count; n++)
            {
                string s = n % columnsPerRow == 0 ? $"),(@p{n}" : $",@p{n}";
                //builds the values in this form: (@p0,@p1,@p2),(@p3,@p4,@p5),....
                //each row is surrounded by round brackets and the column-related parameters are comma separated
                sb.Append(s);
            }
            sb.Append(')');
            string sql = $"INSERT INTO {tableName} ({string.Join(',', colNames)}) VALUES " + sb.ToString();
            return sql;
        }

    }
}
