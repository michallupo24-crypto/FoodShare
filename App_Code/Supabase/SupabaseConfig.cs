using System.Configuration;

public static class SupabaseConfig
{
    public static string Url => ConfigurationManager.AppSettings["SupabaseUrl"];
    public static string AnonKey => ConfigurationManager.AppSettings["SupabaseAnonKey"];
}
