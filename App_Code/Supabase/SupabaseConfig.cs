using System.Configuration;
using System.Net;

public static class SupabaseConfig
{
    static SupabaseConfig()
    {
        // .NET Framework 4.8 defaults ServicePointManager to older SSL/TLS versions on some hosts;
        // Supabase requires TLS 1.2, so force it before any HttpClient call is made.
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }

    public static string Url
    {
        get { return ConfigurationManager.AppSettings["SupabaseUrl"]; }
    }

    public static string AnonKey
    {
        get { return ConfigurationManager.AppSettings["SupabaseAnonKey"]; }
    }
}
