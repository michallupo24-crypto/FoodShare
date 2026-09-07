using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        bool loggedAdmin = Session["isAdmin"] != null && (bool)Session["isAdmin"] == true;
        bool loggedUser = Session["isUser"] != null && (bool)Session["isUser"] == true;

        if (loggedAdmin || loggedUser)
        {
            Session["message"] = "לא ניתן להתחבר כשאתם כבר מחוברים. התנתקו כדי להתחבר לחשבון אחר.";
            Response.Redirect("Message.aspx");
            return;
        }

        if (Request.Form["mySubmit"] == null)
            return;

        string userName = Request.Form["UserName"];
        string password = Request.Form["Password"];

        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(
            "SELECT UserName, isAdmin, id, loginCount, Password FROM Users WHERE UserName = ?",
            new OleDbParameter("UserName", OleDbType.VarWChar) { Value = userName });

        bool found = false;
        if (dt.Rows.Count > 0)
        {
            string storedPassword = dt.Rows[0]["Password"].ToString();

            if (PasswordHasher.IsHashed(storedPassword))
            {
                found = PasswordHasher.Verify(password, storedPassword);
            }
            else if (storedPassword == password)
            {
                // חשבון ישן עם סיסמה בטקסט גלוי - מאמתים ומעדכנים בשקט להצפנה
                found = true;
                MyAdoHelperAccess.ExecuteNonQuery(
                    "UPDATE Users SET Password = ? WHERE id = ?",
                    new OleDbParameter("Password", OleDbType.VarWChar) { Value = PasswordHasher.Hash(password) },
                    new OleDbParameter("id", OleDbType.Integer) { Value = Convert.ToInt32(dt.Rows[0]["id"]) });
            }
        }

        if (found)
        {
            bool isAdmin = Convert.ToBoolean(dt.Rows[0]["isAdmin"]);
            int userId = Convert.ToInt32(dt.Rows[0]["id"]);

            int oldCount = 0;
            if (dt.Rows[0]["loginCount"] != DBNull.Value)
                oldCount = Convert.ToInt32(dt.Rows[0]["loginCount"]);

            int newCount = oldCount + 1;
            MyAdoHelperAccess.ExecuteNonQuery(
                "UPDATE Users SET loginCount = ? WHERE id = ?",
                new OleDbParameter("loginCount", OleDbType.Integer) { Value = newCount },
                new OleDbParameter("id", OleDbType.Integer) { Value = userId });

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
