using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class EditUser : System.Web.UI.Page
{
    private string userId;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "EditUser.aspx");
        UserAuth.RequireAdmin(Session);

        userId = Request.QueryString["id"];
        if (string.IsNullOrEmpty(userId))
        {
            Response.Redirect("AdminPanel.aspx");
            return;
        }

        if (!IsPostBack)
            LoadUser();
    }

    private void LoadUser()
    {
        List<Dictionary<string, object>> rows = SupabaseRest.Select(
            "profiles", "id=eq." + userId + "&select=*", UserAuth.AccessToken(Session));

        if (rows.Count == 0)
        {
            Response.Redirect("AdminPanel.aspx");
            return;
        }

        Dictionary<string, object> r = rows[0];
        txtFirstName.Text = r["first_name"].ToString();
        txtLastName.Text = r["last_name"].ToString();
        txtUserName.Text = r["username"].ToString();
        txtPhonePrefix.Text = r["phone_prefix"] != null ? r["phone_prefix"].ToString() : "";
        txtPhoneNumber.Text = r["phone_number"] != null ? r["phone_number"].ToString() : "";
        txtBirthYear.Text = r["birth_year"] != null ? r["birth_year"].ToString() : "";
        txtGender.Text = r["gender"] != null ? r["gender"].ToString() : "";
        txtCity.Text = r["city"] != null ? r["city"].ToString() : "";
        txtLoginCount.Text = r["login_count"] != null ? r["login_count"].ToString() : "0";
        chkIsAdmin.Checked = r["is_admin"] != null && Convert.ToBoolean(r["is_admin"]);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string token = UserAuth.AccessToken(Session);

        var patch = new Dictionary<string, object>
        {
            { "first_name", txtFirstName.Text.Trim() },
            { "last_name", txtLastName.Text.Trim() },
            { "username", txtUserName.Text.Trim() },
            { "phone_prefix", txtPhonePrefix.Text.Trim() },
            { "phone_number", txtPhoneNumber.Text.Trim() },
            { "birth_year", Convert.ToInt32(txtBirthYear.Text) },
            { "gender", txtGender.Text.Trim() },
            { "city", txtCity.Text.Trim() }
        };

        SupabaseRest.Update("profiles", "id=eq." + userId, patch, token);

        // is_admin נעול מ-UPDATE ישיר (RLS + column grant) ומשתנה רק דרך הפונקציה הזו,
        // כדי שאף משתמש/ת לא יוכל/תוכל לקדם את עצמו/ה למנהל/ת
        SupabaseRest.Rpc("set_admin_status",
            new Dictionary<string, object> { { "target_user", userId }, { "new_value", chkIsAdmin.Checked } },
            token);

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המשתמש עודכן בהצלחה.";
    }
}
