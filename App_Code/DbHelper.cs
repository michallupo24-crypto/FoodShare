using System.Configuration;
using System.Web;

public static class DbHelper
{
    public static string ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["FoodShareDB"].ConnectionString
                .Replace("|DataDirectory|", HttpContext.Current.Server.MapPath("~/App_Data/"));
        }
    }
}
