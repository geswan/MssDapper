namespace DataAccess;
//This is used for dynamic updating of the connection strings
//The property names must match exactly the Connections in appsettings.json
public class ServerOptions
{
    public const string ConnectionStrings = "ConnectionStrings";

    public string MsSql { get; set; } = string.Empty;
    public string MySql { get; set; } = string.Empty;
}