using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

public partial class AddItem : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "AddItem.aspx");

        if (!IsPostBack)
            LoadCategoriesFromXml();
    }

    private void LoadCategoriesFromXml()
    {
        string xmlPath = Server.MapPath("~/App_Data/Categories.xml");
        XmlDocument doc = new XmlDocument();
        using (StreamReader reader = new StreamReader(xmlPath, Encoding.UTF8))
        {
            doc.Load(reader);
        }

        ddlCategory.Items.Clear();
        foreach (XmlNode node in doc.SelectNodes("//Category"))
        {
            ddlCategory.Items.Add(node.InnerText);
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (!UserAuth.IsLoggedIn(Session))
        {
            lblMessage.Text = "יש להתחבר לפני הוספת מוצר.";
            return;
        }

        var item = new Dictionary<string, object>
        {
            { "user_id", UserAuth.UserId(Session) },
            { "item_name", txtItemName.Text.Trim() },
            { "expiry_date", DateTime.Parse(txtExpiry.Text).ToString("yyyy-MM-dd") },
            { "quantity", Convert.ToInt32(txtQuantity.Text) },
            { "category", ddlCategory.SelectedValue },
            { "pickup_city", ddlCity.SelectedValue },
            { "pickup_location", txtLocation.Text.Trim() }
        };

        SupabaseRest.Insert("food_items", item, UserAuth.AccessToken(Session));

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המוצר פורסם בהצלחה!";
        txtItemName.Text = "";
        txtQuantity.Text = "";
        txtLocation.Text = "";
    }
}
