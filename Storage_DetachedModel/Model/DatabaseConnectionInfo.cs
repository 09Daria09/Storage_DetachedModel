internal static class DatabaseConnectionInfo
{
    public static string UserName { get; set; } = "6E7PN2T";
    public static string DatabaseName { get; set; } = "Storage";

    public static string GetConnectionString()
    {
        return $"Initial Catalog={DatabaseName};Data Source={UserName};Integrated Security=true";
    }
}
