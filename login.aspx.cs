using System;
using System.Data;
using System.Web.UI;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool loggedAdmin = Session["isAdmin"] != null && (bool)Session["isAdmin"] == true;
        bool loggedUser = Session["isUser"] != null && (bool)Session["isUser"] == true;

        if (loggedAdmin || loggedUser)
        {
            Session["message"] = "אינך יכולה להתחבר כשאת כבר מחוברת. התנתקי כדי להתחבר לחשבון אחר.";
            Response.Redirect("Message.aspx");
            return;
        }

        if (Request.Form["mySubmit"] == null)
            return;

        string userName = Request.Form["UserName"];
        string password = Request.Form["Password"];

        string sqlSelectQuery = string.Format(
            "SELECT UserName, isAdmin, id, loginCount FROM Users WHERE (UserName = '{0}' AND Password = '{1}')",
            userName.Replace("'", "''"),
            password.Replace("'", "''"));

        bool found = MyAdoHelperAccess.IsExist(sqlSelectQuery);

        if (found)
        {
            DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sqlSelectQuery);
            bool isAdmin = Convert.ToBoolean(dt.Rows[0]["isAdmin"]);
            int userId = Convert.ToInt32(dt.Rows[0]["id"]);

            int oldCount = 0;
            if (dt.Rows[0]["loginCount"] != DBNull.Value)
                oldCount = Convert.ToInt32(dt.Rows[0]["loginCount"]);

            int newCount = oldCount + 1;
            MyAdoHelperAccess.ExecuteNonQuery(
                string.Format("UPDATE Users SET loginCount = {0} WHERE id = {1}", newCount, userId));

            Session["userName"] = userName;
            Session["UserID"] = userId.ToString();
            Session["loginCount"] = newCount;

            if (isAdmin)
            {
                Session["isAdmin"] = true;
                Session["isUser"] = false;
            }
            else
            {
                Session["isUser"] = true;
                Session["isAdmin"] = false;
            }

            Session["message"] = "ההתחברות בהצלחה! זו כניסה מספר " + newCount + ".<br/><br/><a href='HomePage.aspx'>לדף הבית</a>";
        }
        else
        {
            Session["isUser"] = false;
            Session["isAdmin"] = false;
            Session["message"] = "לא נמצא חשבונך.<br/><br/><a href='regestaration.aspx'>להרשמה</a>";
        }

        Response.Redirect("Message.aspx");
    }
}
