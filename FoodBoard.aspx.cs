using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public class FoodItemView
{
    public string ItemId { get; set; }
    public string UserId { get; set; }
    public string ItemName { get; set; }
    public string Category { get; set; }
    public string PickupCity { get; set; }
    public string PickupLocation { get; set; }
    public string Distance { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int DaysLeft { get; set; }
    public string Quantity { get; set; }
    public string UserName { get; set; }
    public string PhotoUrl { get; set; }

    public string Initials
    {
        get { return string.IsNullOrEmpty(UserName) ? "" : UserName.Substring(0, Math.Min(2, UserName.Length)); }
    }

    public string MetaLine
    {
        get { return Quantity + " · " + PickupCity + " · " + PickupLocation + " · " + Distance; }
    }

    public bool IsUrgent
    {
        get { return DaysLeft <= 1; }
    }

    public string ExpiryLabel
    {
        get
        {
            if (DaysLeft <= 0) return "היום";
            if (DaysLeft == 1) return "מחר";
            return "נותרו " + DaysLeft + " ימים";
        }
    }

    public bool HasPhoto { get { return !string.IsNullOrEmpty(PhotoUrl); } }
}

public partial class FoodBoard : System.Web.UI.Page
{
    private List<FoodItemView> currentItems;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadCityFilter();
            LoadCategoryFilter();

            string catFromUrl = Request.QueryString["cat"];
            string cityFromUrl = Request.QueryString["city"];
            bool hasUrlFilter = false;

            if (!string.IsNullOrEmpty(catFromUrl) && ddlCategory.Items.FindByValue(catFromUrl) != null)
            {
                ddlCategory.SelectedValue = catFromUrl;
                hasUrlFilter = true;
            }
            if (!string.IsNullOrEmpty(cityFromUrl) && ddlCity.Items.FindByValue(cityFromUrl) != null)
            {
                ddlCity.SelectedValue = cityFromUrl;
                hasUrlFilter = true;
            }

            LoadItems(hasUrlFilter ? BuildSearchQuery() : DefaultQuery());
        }
    }

    private void LoadCityFilter()
    {
        ddlCity.Items.Clear();
        ddlCity.Items.Add(new ListItem("הכל", ""));
        ddlCity.Items.Add(new ListItem("תל אביב", "Tel Aviv"));
        ddlCity.Items.Add(new ListItem("ירושלים", "Jerusalem"));
        ddlCity.Items.Add(new ListItem("חיפה", "Haifa"));
        ddlCity.Items.Add(new ListItem("באר שבע", "Beersheba"));
    }

    private void LoadCategoryFilter()
    {
        ddlCategory.Items.Clear();
        ddlCategory.Items.Add(new ListItem("הכל", ""));

        string xmlPath = Server.MapPath("~/App_Data/Categories.xml");
        XmlDocument doc = new XmlDocument();
        using (StreamReader reader = new StreamReader(xmlPath, Encoding.UTF8))
        {
            doc.Load(reader);
        }
        foreach (XmlNode node in doc.SelectNodes("//Category"))
        {
            ddlCategory.Items.Add(new ListItem(node.InnerText, node.InnerText));
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        LoadItems(BuildSearchQuery());
        lblMessage.Text = "הסינון בוצע.";
    }

    protected void btnShowAll_Click(object sender, EventArgs e)
    {
        ddlCity.SelectedIndex = 0;
        ddlCategory.SelectedIndex = 0;
        ddlExpiry.SelectedIndex = 0;
        lblMessage.Text = "";
        LoadItems(DefaultQuery());
    }

    private string DefaultQuery()
    {
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        return "select=*,profiles(username)&expiry_date=gte." + today + "&order=expiry_date.asc";
    }

    private string BuildSearchQuery()
    {
        string today = DateTime.Today.ToString("yyyy-MM-dd");
        string query = "select=*,profiles(username)&expiry_date=gte." + today;

        if (ddlCity.SelectedValue != "")
            query += "&pickup_city=eq." + Uri.EscapeDataString(ddlCity.SelectedValue);

        if (ddlCategory.SelectedValue != "")
            query += "&category=eq." + Uri.EscapeDataString(ddlCategory.SelectedValue);

        if (ddlExpiry.SelectedValue != "")
        {
            int days = Convert.ToInt32(ddlExpiry.SelectedValue);
            string maxDate = DateTime.Today.AddDays(days).ToString("yyyy-MM-dd");
            query += "&expiry_date=lte." + maxDate;
        }

        query += "&order=expiry_date.asc";
        return query;
    }

    private void LoadItems(string query)
    {
        List<Dictionary<string, object>> rows = SupabaseRest.Select("food_items", query, UserAuth.AccessToken(Session));
        string viewerCity = UserAuth.IsLoggedIn(Session) ? UserAuth.UserCity(Session) : null;

        currentItems = new List<FoodItemView>();

        foreach (Dictionary<string, object> row in rows)
        {
            DateTime expiry = Convert.ToDateTime(row["expiry_date"]);
            int daysLeft = (expiry.Date - DateTime.Today).Days;

            string userName = "";
            object profilesObj;
            row.TryGetValue("profiles", out profilesObj);
            Dictionary<string, object> profile = profilesObj as Dictionary<string, object>;
            if (profile != null && profile.ContainsKey("username"))
                userName = profile["username"].ToString();

            string pickupCity = row["pickup_city"].ToString();
            double? distance = GeoHelper.DistanceKm(viewerCity, pickupCity);
            string distanceText = distance.HasValue ? Math.Round(distance.Value) + " ק\"מ" : "מרחק לא ידוע";

            bool photoDisabled = row.ContainsKey("photo_disabled") && Convert.ToBoolean(row["photo_disabled"]);
            string photoUrl = (!photoDisabled && row.ContainsKey("photo_url") && row["photo_url"] != null)
                ? row["photo_url"].ToString() : "";

            currentItems.Add(new FoodItemView
            {
                ItemId = row["id"].ToString(),
                UserId = row["user_id"].ToString(),
                ItemName = row["item_name"].ToString(),
                Category = row["category"].ToString(),
                PickupCity = pickupCity,
                PickupLocation = GeoHelper.PartialLocation(row["pickup_location"].ToString()),
                Distance = distanceText,
                ExpiryDate = expiry,
                DaysLeft = daysLeft,
                Quantity = row["quantity"].ToString(),
                UserName = userName,
                PhotoUrl = photoUrl
            });
        }

        rptItems.DataSource = currentItems;
        rptItems.DataBind();
        lblEmpty.Visible = currentItems.Count == 0;
    }

    public bool CanEdit(object itemUserId)
    {
        if (!UserAuth.IsLoggedIn(Session))
            return false;
        if (UserAuth.IsAdmin(Session))
            return true;
        return UserAuth.UserId(Session) == itemUserId.ToString();
    }

    public bool CanMessage(object itemUserId)
    {
        if (!UserAuth.IsLoggedIn(Session))
            return false;
        return UserAuth.UserId(Session) != itemUserId.ToString();
    }

    public bool CanReportPhoto(object itemUserId, object photoUrl)
    {
        if (!UserAuth.IsLoggedIn(Session))
            return false;
        if (photoUrl == null || photoUrl.ToString() == "")
            return false;
        return UserAuth.UserId(Session) != itemUserId.ToString();
    }

    protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (!UserAuth.IsLoggedIn(Session))
        {
            Response.Redirect("login.aspx?returnUrl=FoodBoard.aspx");
            return;
        }

        string itemId = e.CommandArgument.ToString();

        if (e.CommandName == "DeleteItem")
        {
            SupabaseRest.Delete("food_items", "id=eq." + itemId, UserAuth.AccessToken(Session));
            lblMessage.Text = "המוצר נמחק.";
            LoadItems(BuildSearchQuery());
        }
        else if (e.CommandName == "ReportPhoto")
        {
            var report = new Dictionary<string, object>
            {
                { "item_id", Convert.ToInt64(itemId) },
                { "reporter_id", UserAuth.UserId(Session) },
                { "reason", "דיווח מלוח המודעות" }
            };

            try
            {
                SupabaseRest.Insert("photo_reports", report, UserAuth.AccessToken(Session));
                lblMessage.Text = "התמונה דווחה והוסתרה לבדיקה.";
            }
            catch (Exception)
            {
                lblMessage.Text = "לא ניתן לדווח על התמונה הזו (אולי כבר דיווחתם עליה).";
            }

            LoadItems(BuildSearchQuery());
        }
    }
}
