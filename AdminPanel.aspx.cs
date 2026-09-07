using System;
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
        }
    }

    private void LoadUsers()
    {
        string sql = "SELECT id, UserName, FirstName, LastName, Email, City, isAdmin, loginCount FROM Users ORDER BY id";
        gvUsers.DataSource = MyAdoHelperAccess.ExecuteDataTable(sql);
        gvUsers.DataBind();
    }

    protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "DelUser")
            return;

        int userId = Convert.ToInt32(e.CommandArgument);


        MyAdoHelperAccess.ExecuteNonQuery("DELETE FROM FoodItems WHERE UserID = " + userId);
        MyAdoHelperAccess.ExecuteNonQuery("DELETE FROM Users WHERE id = " + userId);

        lblMessage.Text = "המשתמש נמחק בהצלחה.";
        LoadUsers();
    }
}