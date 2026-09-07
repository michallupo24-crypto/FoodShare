using System;
using System.Data;
using System.Web.UI;

public partial class EditUser : System.Web.UI.Page
{
    private int userId;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "EditUser.aspx");
        UserAuth.RequireAdmin(Session);

        if (!int.TryParse(Request.QueryString["id"], out userId))
        {
            Response.Redirect("AdminPanel.aspx");
            return;
        }

        if (!IsPostBack)
            LoadUser();
    }

    private void LoadUser()
    {
        string sql = "SELECT * FROM Users WHERE id = " + userId;
        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql);
        if (dt.Rows.Count == 0)
        {
            Response.Redirect("AdminPanel.aspx");
            return;
        }

        DataRow r = dt.Rows[0];
        txtFirstName.Text = r["FirstName"].ToString();
        txtLastName.Text = r["LastName"].ToString();
        txtUserName.Text = r["UserName"].ToString();
        txtEmail.Text = r["Email"].ToString();
        txtPassword.Text = r["Password"].ToString();
        txtPhonePrefix.Text = r["PhonePrefix"].ToString();
        txtPhoneNumber.Text = r["PhoneNumber"].ToString();
        txtBirthYear.Text = r["BirthYear"].ToString();
        txtGender.Text = r["Gender"].ToString();
        txtCity.Text = r["City"].ToString();
        txtLoginCount.Text = r["loginCount"] == DBNull.Value ? "0" : r["loginCount"].ToString();
        chkIsAdmin.Checked = r["isAdmin"] != DBNull.Value && Convert.ToBoolean(r["isAdmin"]);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string isAdminVal = chkIsAdmin.Checked ? "True" : "False";

        string sql = string.Format(
            "UPDATE Users SET [FirstName]='{0}', [LastName]='{1}', [UserName]='{2}', [Email]='{3}', [Password]='{4}', " +
            "[PhonePrefix]='{5}', [PhoneNumber]='{6}', [BirthYear]={7}, [Gender]='{8}', [City]='{9}', " +
            "[loginCount]={10}, [isAdmin]={11} WHERE id={12}",
            txtFirstName.Text.Replace("'", "''"),
            txtLastName.Text.Replace("'", "''"),
            txtUserName.Text.Replace("'", "''"),
            txtEmail.Text.Replace("'", "''"),
            txtPassword.Text.Replace("'", "''"),
            txtPhonePrefix.Text.Replace("'", "''"),
            txtPhoneNumber.Text.Replace("'", "''"),
            txtBirthYear.Text,
            txtGender.Text.Replace("'", "''"),
            txtCity.Text.Replace("'", "''"),
            txtLoginCount.Text,
            isAdminVal,
            userId);

        MyAdoHelperAccess.ExecuteNonQuery(sql);

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המשתמש עודכן בהצלחה.";
    }
}
