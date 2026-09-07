using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

public partial class AddItem : System.Web.UI.Page
{
    protected bool CanUploadPhoto;
    protected int ReviewCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "AddItem.aspx");
        Form.Enctype = "multipart/form-data";

        try
        {
            ReviewCount = ReviewHelper.GetReviewCount(UserAuth.UserId(Session), UserAuth.AccessToken(Session));
            CanUploadPhoto = ReviewCount >= ReviewHelper.MinReviewsForPhoto;
        }
        catch (Exception)
        {
            CanUploadPhoto = false;
        }

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

        string token = UserAuth.AccessToken(Session);
        string userId = UserAuth.UserId(Session);

        double lat = 0, lon = 0;
        bool hasLocation = double.TryParse(hdnLat.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lat) &&
            double.TryParse(hdnLon.Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lon);

        if (!hasLocation && string.IsNullOrWhiteSpace(txtLocation.Text))
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "יש למלא כתובת, או לשתף מיקום GPS.";
            return;
        }

        var item = new Dictionary<string, object>
        {
            { "user_id", userId },
            { "item_name", txtItemName.Text.Trim() },
            { "expiry_date", DateTime.Parse(txtExpiry.Text).ToString("yyyy-MM-dd") },
            { "quantity", Convert.ToInt32(txtQuantity.Text) },
            { "category", ddlCategory.SelectedValue },
            { "pickup_city", ddlCity.SelectedValue },
            { "pickup_location", txtLocation.Text.Trim() }
        };

        if (hasLocation)
        {
            item["lat"] = lat;
            item["lon"] = lon;
        }

        if (CanUploadPhoto && fuPhoto.HasFile)
        {
            try
            {
                string path = userId + "/" + Guid.NewGuid() + "-" + Path.GetFileName(fuPhoto.FileName);
                string photoUrl = SupabaseStorage.Upload("item-photos", path, fuPhoto.FileBytes, fuPhoto.PostedFile.ContentType, token);
                item["photo_url"] = photoUrl;
            }
            catch (Exception)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "המוצר לא פורסם - העלאת התמונה נכשלה. נסו שוב בלי תמונה או עם קובץ אחר.";
                return;
            }
        }

        try
        {
            SupabaseRest.Insert("food_items", item, token);
        }
        catch (Exception)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "פרסום המוצר נכשל.";
            return;
        }

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המוצר פורסם בהצלחה!";
        txtItemName.Text = "";
        txtQuantity.Text = "";
        txtLocation.Text = "";
    }
}
