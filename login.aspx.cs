using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (UserAuth.IsLoggedIn(Session))
        {
            Session["message"] = "לא ניתן להתחבר כשאתם כבר מחוברים. התנתקו כדי להתחבר לחשבון אחר.";
            Response.Redirect("Message.aspx");
            return;
        }

        if (Request.Form["mySubmit"] == null)
            return;

        string email = Request.Form["Email"];
        string password = Request.Form["Password"];

        SupabaseAuthResult result = SupabaseAuth.SignIn(email, password);

        if (!result.Success)
        {
            Session["message"] = "ההתחברות נכשלה: " + result.ErrorMessage + "<br/><br/><a href='regestaration.aspx'>להרשמה</a>";
            Response.Redirect("Message.aspx");
            return;
        }

        List<Dictionary<string, object>> profiles = SupabaseRest.Select(
            "profiles",
            "id=eq." + result.UserId + "&select=*",
            result.AccessToken);

        if (profiles.Count == 0)
        {
            Session["message"] = "החשבון קיים אך פרופיל המשתמש חסר. פנו למנהל/ת האתר.";
            Response.Redirect("Message.aspx");
            return;
        }

        Dictionary<string, object> profile = profiles[0];
        int newCount = Convert.ToInt32(profile["login_count"]) + 1;
        SupabaseRest.Rpc("increment_login_count", null, result.AccessToken);

        Session["SupabaseUserId"] = result.UserId;
        Session["SupabaseAccessToken"] = result.AccessToken;
        Session["userName"] = profile["username"].ToString();
        Session["isAdmin"] = Convert.ToBoolean(profile["is_admin"]);
        Session["loginCount"] = newCount;

        Session["message"] = "ההתחברות בהצלחה! זו כניסה מספר " + newCount + ".<br/><br/><a href='HomePage.aspx'>לדף הבית</a>";
        Response.Redirect("Message.aspx");
    }
}
