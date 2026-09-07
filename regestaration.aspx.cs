using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class regestaration : System.Web.UI.Page
{
    public string msg = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (UserAuth.IsLoggedIn(Session))
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

        SupabaseAuthResult result = SupabaseAuth.SignUp(email, password);

        if (!result.Success)
        {
            msg = "ההרשמה נכשלה: " + result.ErrorMessage;
        }
        else
        {
            var profile = new Dictionary<string, object>
            {
                { "id", result.UserId },
                { "username", userName },
                { "first_name", firstName },
                { "last_name", lastName },
                { "phone_prefix", phonePrefix },
                { "phone_number", phoneNumber },
                { "birth_year", birthYear },
                { "gender", gender },
                { "city", city }
            };

            double lat, lon;
            if (double.TryParse(Request.Form["Lat"], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lat) &&
                double.TryParse(Request.Form["Lon"], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lon))
            {
                profile["lat"] = lat;
                profile["lon"] = lon;
            }

            try
            {
                SupabaseRest.Insert("profiles", profile, result.AccessToken);
                msg = "ברוכים הבאים לאתר! נרשמת בהצלחה.";
            }
            catch (Exception)
            {
                msg = "החשבון נוצר אך שמירת פרטי הפרופיל נכשלה (ייתכן ששם המשתמש כבר תפוס). נסו שם משתמש אחר או פנו למנהל/ת האתר.";
            }
        }

        msg += "<br/><br/><a href='login.aspx'>להתחברות</a>";
        Session["message"] = msg;
        Response.Redirect("Message.aspx");
    }
}
