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
            Session["message"] = "אינך יכול להירשם כשאתה מחובר. התנתק קודם.";
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
        string idification = Request.Form["idification"];

        string sqlCheck = string.Format(
            "SELECT UserName FROM Users WHERE UserName = '{0}'",
            userName.Replace("'", "''"));

        if (MyAdoHelperAccess.IsExist(sqlCheck))
        {
            msg = "שם משתמש זה תפוס. בחרי שם אחר.";
        }
        else
        {
            string sqlSignup = string.Format(
                "INSERT INTO Users ([FirstName], [LastName], [UserName], [Email], [Password], [BirthYear], [Gender], [PhonePrefix], [PhoneNumber], [City], [idification], [isAdmin], [loginCount]) " +
                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', {5}, '{6}', '{7}', '{8}', '{9}', '{10}', No, 0)",
                firstName.Replace("'", "''"),
                lastName.Replace("'", "''"),
                userName.Replace("'", "''"),
                email.Replace("'", "''"),
                password.Replace("'", "''"),
                birthYear,
                gender.Replace("'", "''"),
                phonePrefix.Replace("'", "''"),
                phoneNumber.Replace("'", "''"),
                city.Replace("'", "''"),
                idification.Replace("'", "''"));

            MyAdoHelperAccess.ExecuteNonQuery(sqlSignup);
            msg = "ברוכים הבאים לאתר! נרשמת בהצלחה.";
        }

        msg += "<br/><br/><a href='login.aspx'>להתחברות</a>";
        Session["message"] = msg;
        Response.Redirect("Message.aspx");
    }
}
