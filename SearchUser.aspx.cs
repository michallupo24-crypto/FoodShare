using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

public partial class SearchUser : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "SearchUser.aspx");
        UserAuth.RequireAdmin(Session);

        if (!IsPostBack)
        {
            LoadUsers(null, null);
            UpdateStatistics();
        }
    }

    private void LoadUsers(string sql, OleDbParameter[] parameters)
    {
        if (string.IsNullOrEmpty(sql))
        {
            sql = "SELECT id, UserName, FirstName, LastName, City, Email, loginCount FROM Users ORDER BY UserName";
            parameters = new OleDbParameter[0];
        }

        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql, parameters);
        gvResults.DataSource = dt;
        gvResults.DataBind();
    }

    private void UpdateStatistics()
    {
        DataTable dtMichal = MyAdoHelperAccess.ExecuteDataTable(
            "SELECT COUNT(*) FROM Users WHERE FirstName = ?",
            new OleDbParameter("FirstName", OleDbType.VarWChar) { Value = "מיכל" });
        int countMichal = Convert.ToInt32(dtMichal.Rows[0][0]);

        DataTable dtTelAviv = MyAdoHelperAccess.ExecuteDataTable(
            "SELECT COUNT(*) FROM Users WHERE City = ?",
            new OleDbParameter("City", OleDbType.VarWChar) { Value = "Tel Aviv" });
        int countTelAviv = Convert.ToInt32(dtTelAviv.Rows[0][0]);

        specificquestions.Text = "מספר המשתמשות בשם מיכל: " + countMichal + " | מספר המשתמשים בתל אביב: " + countTelAviv;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string sql = "SELECT id, UserName, FirstName, LastName, City, Email, loginCount FROM Users WHERE 1=1";
        var parameters = new System.Collections.Generic.List<OleDbParameter>();

        if (!string.IsNullOrEmpty(txtSearchName.Text.Trim()))
        {
            sql += " AND UserName LIKE ?";
            parameters.Add(new OleDbParameter("UserName", OleDbType.VarWChar) { Value = "%" + txtSearchName.Text.Trim() + "%" });
        }

        if (ddlCity.SelectedValue != "")
        {
            sql += " AND City = ?";
            parameters.Add(new OleDbParameter("City", OleDbType.VarWChar) { Value = ddlCity.SelectedValue });
        }

        sql += " ORDER BY UserName";

        LoadUsers(sql, parameters.ToArray());
        UpdateStatistics();
        lblMessage.Text = "תוצאות חיפוש:";
    }

    protected void btnShowAll_Click(object sender, EventArgs e)
    {
        txtSearchName.Text = "";
        ddlCity.SelectedIndex = 0;
        lblMessage.Text = "";
        LoadUsers(null, null);
        UpdateStatistics();
    }
}
