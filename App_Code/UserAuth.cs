using System.Web;
using System.Web.SessionState;

public static class UserAuth
{
    public static bool IsLoggedIn(HttpSessionState session)
    {
        return session["SupabaseUserId"] != null;
    }

    public static bool IsAdmin(HttpSessionState session)
    {
        return session["isAdmin"] != null && (bool)session["isAdmin"];
    }

    public static string UserId(HttpSessionState session)
    {
        return session["SupabaseUserId"] as string;
    }

    public static string AccessToken(HttpSessionState session)
    {
        return session["SupabaseAccessToken"] as string;
    }

    public static void RequireLogin(HttpSessionState session, string returnUrl)
    {
        if (!IsLoggedIn(session))
        {
            HttpContext.Current.Response.Redirect("login.aspx?returnUrl=" +
                HttpContext.Current.Server.UrlEncode(returnUrl));
        }
    }

    public static void RequireAdmin(HttpSessionState session)
    {
        if (!IsAdmin(session))
            HttpContext.Current.Response.Redirect("HomePage.aspx");
    }
}
