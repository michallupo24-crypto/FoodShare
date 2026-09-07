using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

public partial class SearchUser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "SearchUser.aspx");
        UserAuth.RequireAdmin(Session);

        if (!IsPostBack)
        {
            LoadUsers(null);
            UpdateStatistics();
        }
    }

    // ה-GridView נשאר בלי שינוי - בונים DataTable מקומית באותם שמות עמודות שהיו מול ה-Access
    private void LoadUsers(string extraQuery)
    {
        string query = "select=id,username,first_name,last_name,city,login_count&order=username";
        if (!string.IsNullOrEmpty(extraQuery))
            query += "&" + extraQuery;

        List<Dictionary<string, object>> rows = SupabaseRest.Select("profiles", query, UserAuth.AccessToken(Session));

        DataTable table = new DataTable();
        table.Columns.Add("id", typeof(string));
        table.Columns.Add("UserName", typeof(string));
        table.Columns.Add("FirstName", typeof(string));
        table.Columns.Add("LastName", typeof(string));
        table.Columns.Add("City", typeof(string));
        table.Columns.Add("loginCount", typeof(int));

        foreach (Dictionary<string, object> row in rows)
        {
            table.Rows.Add(
                row["id"].ToString(),
                row["username"].ToString(),
                row["first_name"].ToString(),
                row["last_name"].ToString(),
                row["city"] != null ? row["city"].ToString() : "",
                Convert.ToInt32(row["login_count"]));
        }

        gvResults.DataSource = table;
        gvResults.DataBind();
    }

    private void UpdateStatistics()
    {
        List<Dictionary<string, object>> michal = SupabaseRest.Select(
            "profiles", "select=id&first_name=eq." + Uri.EscapeDataString("מיכל"), UserAuth.AccessToken(Session));

        List<Dictionary<string, object>> telAviv = SupabaseRest.Select(
            "profiles", "select=id&city=eq.Tel Aviv", UserAuth.AccessToken(Session));

        specificquestions.Text = "מספר המשתמשות בשם מיכל: " + michal.Count + " | מספר המשתמשים בתל אביב: " + telAviv.Count;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string query = "";

        if (!string.IsNullOrEmpty(txtSearchName.Text.Trim()))
            query += "username=ilike.*" + Uri.EscapeDataString(txtSearchName.Text.Trim()) + "*";

        if (ddlCity.SelectedValue != "")
            query += (query != "" ? "&" : "") + "city=eq." + Uri.EscapeDataString(ddlCity.SelectedValue);

        LoadUsers(query);
        UpdateStatistics();
        lblMessage.Text = "תוצאות חיפוש:";
    }

    protected void btnShowAll_Click(object sender, EventArgs e)
    {
        txtSearchName.Text = "";
        ddlCity.SelectedIndex = 0;
        lblMessage.Text = "";
        LoadUsers(null);
        UpdateStatistics();
    }
}
