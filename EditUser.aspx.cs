using System;
using System.Data;
using System.Data.OleDb;
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
        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(
            "SELECT * FROM Users WHERE id = ?",
            new OleDbParameter("id", OleDbType.Integer) { Value = userId });

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
        txtPassword.Text = "";
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
        string sql = @"UPDATE Users SET [FirstName]=?, [LastName]=?, [UserName]=?, [Email]=?,
                       [PhonePrefix]=?, [PhoneNumber]=?, [BirthYear]=?, [Gender]=?, [City]=?,
                       [loginCount]=?, [isAdmin]=?";

        if (!string.IsNullOrEmpty(txtPassword.Text))
            sql += ", [Password]=?";

        sql += " WHERE id=?";

        var cmdParams = new System.Collections.Generic.List<OleDbParameter>
        {
            new OleDbParameter("FirstName", OleDbType.VarWChar) { Value = txtFirstName.Text.Trim() },
            new OleDbParameter("LastName", OleDbType.VarWChar) { Value = txtLastName.Text.Trim() },
            new OleDbParameter("UserName", OleDbType.VarWChar) { Value = txtUserName.Text.Trim() },
            new OleDbParameter("Email", OleDbType.VarWChar) { Value = txtEmail.Text.Trim() },
            new OleDbParameter("PhonePrefix", OleDbType.VarWChar) { Value = txtPhonePrefix.Text.Trim() },
            new OleDbParameter("PhoneNumber", OleDbType.VarWChar) { Value = txtPhoneNumber.Text.Trim() },
            new OleDbParameter("BirthYear", OleDbType.Integer) { Value = Convert.ToInt32(txtBirthYear.Text) },
            new OleDbParameter("Gender", OleDbType.VarWChar) { Value = txtGender.Text.Trim() },
            new OleDbParameter("City", OleDbType.VarWChar) { Value = txtCity.Text.Trim() },
            new OleDbParameter("loginCount", OleDbType.Integer) { Value = Convert.ToInt32(txtLoginCount.Text) },
            new OleDbParameter("isAdmin", OleDbType.Boolean) { Value = chkIsAdmin.Checked }
        };

        if (!string.IsNullOrEmpty(txtPassword.Text))
            cmdParams.Add(new OleDbParameter("Password", OleDbType.VarWChar) { Value = PasswordHasher.Hash(txtPassword.Text) });

        cmdParams.Add(new OleDbParameter("id", OleDbType.Integer) { Value = userId });

        MyAdoHelperAccess.ExecuteNonQuery(sql, cmdParams.ToArray());

        txtPassword.Text = "";
        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המשתמש עודכן בהצלחה.";
    }
}
