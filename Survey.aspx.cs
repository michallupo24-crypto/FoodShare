using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Web.UI;

public partial class Survey : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // בדיקה אם המשתמש מחובר (לפי הפונקציה שלך)
        if (Session["UserID"] == null)
        {
            Response.Redirect("login.aspx");
            return;
        }

        if (!IsPostBack) ShowResults();
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        if (rblAnswer.SelectedIndex < 0) { lblMessage.Text = "נא לבחור תשובה"; return; }

        string path = Server.MapPath("~/App_Data/Survey.xml");

        // יצירת הקובץ אם הוא לא קיים
        if (!File.Exists(path))
        {
            new XDocument(new XElement("SurveyResults")).Save(path);
        }

        XDocument doc = XDocument.Load(path);

        // הוספת תשובה
        doc.Root.Add(new XElement("Response",
            new XElement("UserID", Session["UserID"].ToString()),
            new XElement("Answer", rblAnswer.SelectedValue)
        ));

        doc.Save(path);
        lblMessage.Text = "תודה! תשובתך נשמרה.";
        ShowResults();
    }

    private void ShowResults()
    {
        string path = Server.MapPath("~/App_Data/Survey.xml");
        if (!File.Exists(path)) return;

        XDocument doc = XDocument.Load(path);
        var results = doc.Descendants("Response")
                         .GroupBy(r => r.Element("Answer").Value)
                         .Select(g => new { Answer = g.Key, Count = g.Count() });

        string html = "<table border='1' style='direction:rtl; border-collapse:collapse; width:200px;'>";
        html += "<tr><th>תשובה</th><th>כמות</th></tr>";
        foreach (var r in results)
        {
            html += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", r.Answer, r.Count);
        }
        html += "</table>";
        litResults.Text = html;
    }
}