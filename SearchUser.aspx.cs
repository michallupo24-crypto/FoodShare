using System;
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

    private void LoadUsers(string sql)
    {
        if (string.IsNullOrEmpty(sql))
        {
            sql = "SELECT id, UserName, FirstName, LastName, City, Email, loginCount FROM Users ORDER BY UserName";
        }

        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql);
        gvResults.DataSource = dt;
        gvResults.DataBind();
    }

    private void UpdateStatistics()
    {
        string sqlMichal = "SELECT COUNT(*) FROM Users WHERE FirstName = 'מיכל'";
        DataTable dtMichal = MyAdoHelperAccess.ExecuteDataTable(sqlMichal);
        int countMichal = Convert.ToInt32(dtMichal.Rows[0][0]);

        string sqlTelAviv = "SELECT COUNT(*) FROM Users WHERE City = 'Tel Aviv'";
        DataTable dtTelAviv = MyAdoHelperAccess.ExecuteDataTable(sqlTelAviv);
        int countTelAviv = Convert.ToInt32(dtTelAviv.Rows[0][0]);

        specificquestions.Text = "מספר המשתמשות בשם מיכל: " + countMichal + " | מספר המשתמשים בתל אביב: " + countTelAviv;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string sql = "SELECT id, UserName, FirstName, LastName, City, Email, loginCount FROM Users WHERE 1=1";

        if (!string.IsNullOrEmpty(txtSearchName.Text.Trim()))
            sql += " AND UserName LIKE '%" + txtSearchName.Text.Trim().Replace("'", "''") + "%'";

        if (ddlCity.SelectedValue != "")
            sql += " AND City = '" + ddlCity.SelectedValue.Replace("'", "''") + "'";

        sql += " ORDER BY UserName";

        LoadUsers(sql);
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