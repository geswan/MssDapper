namespace DataAccess;

public class ServerOptions
{
    public const string ConnectionStrings = "ConnectionStrings";

    public string MsSql { get; set; } = string.Empty;
    public string MySql { get; set; } = string.Empty;
    public string MariaDb { get; set; } = string.Empty;

}