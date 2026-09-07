using System;
using System.Web.UI;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Charset = "utf-8";
        Response.ContentEncoding = System.Text.Encoding.UTF8;

        cssLink.Href = "StyleSheet.css?v=" + System.IO.File.GetLastWriteTime(Server.MapPath("~/StyleSheet.css")).Ticks;

        bool loggedIn = UserAuth.IsLoggedIn(Session);
        bool isAdmin = UserAuth.IsAdmin(Session);

        lnkRegister.Visible = !loggedIn;
        lnkLogin.Visible = !loggedIn;
        litS4.Visible = !loggedIn;
        litS5.Visible = !loggedIn;

        lnkAddItem.Visible = loggedIn;
        lnkSurvey.Visible = loggedIn;
        lnkLogout.Visible = loggedIn;
        lnkMessages.Visible = loggedIn;
        litS6.Visible = loggedIn;
        litS7.Visible = loggedIn;
        litS8.Visible = loggedIn;
        litS10.Visible = loggedIn;

        lnkAdmin.Visible = isAdmin;
        litS9.Visible = isAdmin;

        if (loggedIn)
        {
            string name = Session["userName"] != null ? Session["userName"].ToString() : "";
            string role = isAdmin ? "מנהל/ת" : "משתמש/ת";
            int loginCount = GetLoginCount();

            lblUserStatus.Text = "שלום, " + name + " (" + role + ") | מספר כניסות: " + loginCount;

            // עטוף ב-try כדי שדף עם תקלה זמנית מול Supabase (או לפני שהרצת את 002_messages.sql)
            // לא יפיל את כל האתר - רק את הבאדג' עצמו
            try
            {
                int unread = GetUnreadCount();
                if (unread > 0)
                {
                    lblUnreadBadge.Text = unread.ToString();
                    lblUnreadBadge.Visible = true;
                }
            }
            catch (Exception)
            {
                lblUnreadBadge.Visible = false;
            }
        }
        else
        {
            lblUserStatus.Text = "אורח/ת – התחברו או הירשמו כדי להוסיף מוצרים";
        }
    }

    private int GetLoginCount()
    {
        return Session["loginCount"] != null ? Convert.ToInt32(Session["loginCount"]) : 0;
    }

    private int GetUnreadCount()
    {
        return (int)SupabaseRest.RpcScalar("unread_message_count", null, UserAuth.AccessToken(Session));
    }
}
