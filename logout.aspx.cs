using System;

public partial class logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Remove("isAdmin");
        Session.Remove("isUser");
        Session.Remove("userName");
        Session.Remove("UserID");
        Session.Remove("loginCount");
        Session.Clear();
        Response.Redirect("FoodBoard.aspx");
    }
}
