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
        {
            LoadUsers();
            LoadReports();
        }
    }

    private void LoadUsers()
    {
        string token = UserAuth.AccessToken(Session);

        List<Dictionary<string, object>> rows;
        try
        {
            rows = SupabaseRest.Select(
                "profiles",
                "select=id,username,first_name,last_name,city,is_admin,login_count,trust_points,is_blocked&order=username",
                token);
        }
        catch (Exception)
        {
            // trust_points / is_blocked עדיין לא קיימים (לפני הרצת 007) - נופלים לשאילתה הבסיסית
            rows = SupabaseRest.Select(
                "profiles",
                "select=id,username,first_name,last_name,city,is_admin,login_count&order=username",
                token);
        }

        Dictionary<string, int> oneStarCounts = new Dictionary<string, int>();
        try
        {
            List<Dictionary<string, object>> oneStars = SupabaseRest.Select(
                "one_star_counts", "select=reviewee_id,one_star_count", token);
            foreach (Dictionary<string, object> r in oneStars)
                oneStarCounts[r["reviewee_id"].ToString()] = Convert.ToInt32(r["one_star_count"]);
        }
        catch (Exception)
        {
            // one_star_counts / trust_points עדיין לא קיימים (לפני הרצת 007) - הטבלה תוצג בלי דגלים
        }

        DataTable table = new DataTable();
        table.Columns.Add("id", typeof(string));
        table.Columns.Add("UserName", typeof(string));
        table.Columns.Add("FirstName", typeof(string));
        table.Columns.Add("LastName", typeof(string));
        table.Columns.Add("City", typeof(string));
        table.Columns.Add("isAdmin", typeof(bool));
        table.Columns.Add("loginCount", typeof(int));
        table.Columns.Add("TrustPoints", typeof(string));
        table.Columns.Add("OneStarCount", typeof(int));
        table.Columns.Add("IsBlocked", typeof(bool));
        table.Columns.Add("Flagged", typeof(bool));

        foreach (Dictionary<string, object> row in rows)
        {
            string id = row["id"].ToString();
            int oneStarCount = oneStarCounts.ContainsKey(id) ? oneStarCounts[id] : 0;
            object trustObj;
            row.TryGetValue("trust_points", out trustObj);
            string trustPoints = trustObj != null ? trustObj.ToString() : "-";
            int trustPointsInt = trustObj != null ? Convert.ToInt32(trustObj) : 100;
            bool isBlocked = row.ContainsKey("is_blocked") && Convert.ToBoolean(row["is_blocked"]);
            bool flagged = !isBlocked && (trustPointsInt <= 50 || oneStarCount > 2);

            table.Rows.Add(
                id,
                row["username"].ToString(),
                row["first_name"].ToString(),
                row["last_name"].ToString(),
                row["city"] != null ? row["city"].ToString() : "",
                Convert.ToBoolean(row["is_admin"]),
                Convert.ToInt32(row["login_count"]),
                trustPoints,
                oneStarCount,
                isBlocked,
                flagged);
        }

        gvUsers.DataSource = table;
        gvUsers.DataBind();
    }

    private void LoadReports()
    {
        try
        {
            List<Dictionary<string, object>> reports = SupabaseRest.Select(
                "photo_reports",
                "select=id,item_id,reason,created_at,food_items(item_name)&status=eq.pending&order=created_at.desc",
                UserAuth.AccessToken(Session));

            DataTable table = new DataTable();
            table.Columns.Add("ReportId", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Reason", typeof(string));
            table.Columns.Add("CreatedAt", typeof(DateTime));

            foreach (Dictionary<string, object> r in reports)
            {
                object itemObj;
                r.TryGetValue("food_items", out itemObj);
                Dictionary<string, object> item = itemObj as Dictionary<string, object>;
                string itemName = item != null && item.ContainsKey("item_name") ? item["item_name"].ToString() : "(מוצר נמחק)";

                table.Rows.Add(
                    r["id"].ToString(),
                    itemName,
                    r["reason"] != null ? r["reason"].ToString() : "",
                    Convert.ToDateTime(r["created_at"]));
            }

            gvReports.DataSource = table;
            gvReports.DataBind();
            pnlReports.Visible = true;
        }
        catch (Exception)
        {
            // photo_reports עדיין לא קיים (לפני הרצת 007) - פשוט מסתירים את הסקשן
            pnlReports.Visible = false;
        }
    }

    protected void gvUsers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;

        DataRowView rowView = e.Row.DataItem as DataRowView;
        if (rowView != null && (bool)rowView["Flagged"])
            e.Row.CssClass += " flagged-row";
    }

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string userId = e.CommandArgument.ToString();
        string token = UserAuth.AccessToken(Session);

        if (e.CommandName == "DelUser")
        {
            SupabaseRest.Delete("food_items", "user_id=eq." + userId, token);
            SupabaseRest.Delete("profiles", "id=eq." + userId, token);
            lblMessage.Text = "פרופיל המשתמש נמחק. שימו לב: חשבון ההתחברות עצמו לא נמחק אוטומטית - יש למחוק אותו ידנית מה-Dashboard של Supabase אם צריך.";
        }
        else if (e.CommandName == "BlockUser" || e.CommandName == "UnblockUser")
        {
            bool blocked = e.CommandName == "BlockUser";
            SupabaseRest.Rpc("set_user_blocked",
                new Dictionary<string, object> { { "target_user", userId }, { "blocked", blocked } }, token);
            lblMessage.Text = blocked ? "המשתמש/ת נחסם/ה." : "החסימה הוסרה.";
        }

        LoadUsers();
    }

    protected void gvReports_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "Uphold" && e.CommandName != "Dismiss")
            return;

        long reportId = Convert.ToInt64(e.CommandArgument);
        bool upheld = e.CommandName == "Uphold";

        SupabaseRest.Rpc("resolve_photo_report",
            new Dictionary<string, object> { { "report_id", reportId }, { "upheld", upheld } },
            UserAuth.AccessToken(Session));

        lblMessage.Text = upheld ? "התלונה אושרה - התמונה נשארת מוסתרת." : "התלונה נדחתה - התמונה שוחזרה.";
        LoadReports();
        LoadUsers();
    }
}
