using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AdminPanel : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "AdminPanel.aspx");
        UserAuth.RequireAdmin(Session);

        if (!IsPostBack)
            LoadUsers();
    }

    // ה-GridView נשאר בלי שינוי - בונים DataTable מקומית באותם שמות עמודות שהיו מול ה-Access
    private void LoadUsers()
    {
        List<Dictionary<string, object>> rows = SupabaseRest.Select(
            "profiles",
            "select=id,username,first_name,last_name,city,is_admin,login_count&order=username",
            UserAuth.AccessToken(Session));

        DataTable table = new DataTable();
        table.Columns.Add("id", typeof(string));
        table.Columns.Add("UserName", typeof(string));
        table.Columns.Add("FirstName", typeof(string));
        table.Columns.Add("LastName", typeof(string));
        table.Columns.Add("City", typeof(string));
        table.Columns.Add("isAdmin", typeof(bool));
        table.Columns.Add("loginCount", typeof(int));

        foreach (Dictionary<string, object> row in rows)
        {
            table.Rows.Add(
                row["id"].ToString(),
                row["username"].ToString(),
                row["first_name"].ToString(),
                row["last_name"].ToString(),
                row["city"] != null ? row["city"].ToString() : "",
                Convert.ToBoolean(row["is_admin"]),
                Convert.ToInt32(row["login_count"]));
        }

        gvUsers.DataSource = table;
        gvUsers.DataBind();
    }

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "DelUser")
            return;

        string userId = e.CommandArgument.ToString();
        string token = UserAuth.AccessToken(Session);

        SupabaseRest.Delete("food_items", "user_id=eq." + userId, token);
        SupabaseRest.Delete("profiles", "id=eq." + userId, token);

        lblMessage.Text = "פרופיל המשתמש נמחק. שימו לב: חשבון ההתחברות עצמו לא נמחק אוטומטית - יש למחוק אותו ידנית מה-Dashboard של Supabase אם צריך.";
        LoadUsers();
    }
}
