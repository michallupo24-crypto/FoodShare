using System.Web;
using System.Web.SessionState;

public static class UserAuth
{
    public static bool IsLoggedIn(HttpSessionState session)
    {
        bool loggedAdmin = session["isAdmin"] != null && (bool)session["isAdmin"];
        bool loggedUser = session["isUser"] != null && (bool)session["isUser"];
        return loggedAdmin || loggedUser;
    }

    public static bool IsAdmin(HttpSessionState session)
    {
        return session["isAdmin"] != null && (bool)session["isAdmin"];
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
