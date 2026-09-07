using System;
using System.Web.UI;

public partial class Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["message"] != null)
            litMessage.Text = Session["message"].ToString();
        else
            litMessage.Text = "אין הודעה להצגה.";
    }
}
