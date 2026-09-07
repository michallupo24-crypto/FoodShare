using System;
using System.Data.OleDb;
using System.Web.UI;

public partial class regestaration : System.Web.UI.Page
{
    public string msg = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        bool loggedAdmin = Session["isAdmin"] != null && (bool)Session["isAdmin"] == true;
        bool loggedUser = Session["isUser"] != null && (bool)Session["isUser"] == true;

        if (loggedAdmin || loggedUser)
        {
            Session["message"] = "לא ניתן להירשם כשאתם כבר מחוברים. התנתקו קודם.";
            Response.Redirect("Message.aspx");
            return;
        }

        if (Request.Form["mySubmit"] == null)
            return;

        string firstName = Request.Form["FirstName"];
        string lastName = Request.Form["LastName"];
        string userName = Request.Form["UserName"];
        string email = Request.Form["Email"];
        string phonePrefix = Request.Form["PhonePrefix"];
        string phoneNumber = Request.Form["PhoneNumber"];
        string password = Request.Form["Password"];
        int birthYear = int.Parse(Request.Form["BirthYear"]);
        string gender = Request.Form["Gender"];
        string city = Request.Form["City"];

        bool exists = MyAdoHelperAccess.IsExist(
            "SELECT UserName FROM Users WHERE UserName = ?",
            new OleDbParameter("UserName", OleDbType.VarWChar) { Value = userName });

        if (exists)
        {
            msg = "שם משתמש זה תפוס. בחרו שם אחר.";
        }
        else
        {
            string sqlSignup = @"INSERT INTO Users ([FirstName], [LastName], [UserName], [Email], [Password], [BirthYear], [Gender], [PhonePrefix], [PhoneNumber], [City], [idification], [isAdmin], [loginCount])
                                 VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            MyAdoHelperAccess.ExecuteNonQuery(sqlSignup,
                new OleDbParameter("FirstName", OleDbType.VarWChar) { Value = firstName },
                new OleDbParameter("LastName", OleDbType.VarWChar) { Value = lastName },
                new OleDbParameter("UserName", OleDbType.VarWChar) { Value = userName },
                new OleDbParameter("Email", OleDbType.VarWChar) { Value = email },
                new OleDbParameter("Password", OleDbType.VarWChar) { Value = PasswordHasher.Hash(password) },
                new OleDbParameter("BirthYear", OleDbType.Integer) { Value = birthYear },
                new OleDbParameter("Gender", OleDbType.VarWChar) { Value = gender },
                new OleDbParameter("PhonePrefix", OleDbType.VarWChar) { Value = phonePrefix },
                new OleDbParameter("PhoneNumber", OleDbType.VarWChar) { Value = phoneNumber },
                new OleDbParameter("City", OleDbType.VarWChar) { Value = city },
                new OleDbParameter("idification", OleDbType.VarWChar) { Value = "" },
                new OleDbParameter("isAdmin", OleDbType.Boolean) { Value = false },
                new OleDbParameter("loginCount", OleDbType.Integer) { Value = 0 });

            msg = "ברוכים הבאים לאתר! נרשמת בהצלחה.";
        }

        msg += "<br/><br/><a href='login.aspx'>להתחברות</a>";
        Session["message"] = msg;
        Response.Redirect("Message.aspx");
    }
}
