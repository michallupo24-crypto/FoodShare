using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class FoodBoard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadCityFilter();
            LoadCategoryFilter();

            string catFromUrl = Request.QueryString["cat"];
            if (!string.IsNullOrEmpty(catFromUrl))
            {
                ddlCategory.SelectedValue = catFromUrl;
                LoadItems(BuildSearchQuery());
            }
            else
            {
                LoadItems(DefaultQuery());
            }
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

    // ה-GridView הקיים נשאר בלי שינוי - בונים DataTable מקומית באותם שמות עמודות שהיו מול ה-Access
    private void LoadItems(string query)
    {
        List<Dictionary<string, object>> rows = SupabaseRest.Select("food_items", query, UserAuth.AccessToken(Session));

        DataTable table = new DataTable();
        table.Columns.Add("ItemID", typeof(string));
        table.Columns.Add("UserID", typeof(string));
        table.Columns.Add("ItemName", typeof(string));
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("PickupCity", typeof(string));
        table.Columns.Add("PickupLocation", typeof(string));
        table.Columns.Add("ExpiryDate", typeof(DateTime));
        table.Columns.Add("DaysLeft", typeof(int));
        table.Columns.Add("Quantity", typeof(string));
        table.Columns.Add("UserName", typeof(string));

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

            table.Rows.Add(
                row["id"].ToString(),
                row["user_id"].ToString(),
                row["item_name"].ToString(),
                row["category"].ToString(),
                row["pickup_city"].ToString(),
                row["pickup_location"].ToString(),
                expiry,
                daysLeft,
                row["quantity"].ToString(),
                userName);
        }

        gvItems.DataSource = table;
        gvItems.DataBind();
    }

    public bool CanEdit(object itemUserId)
    {
        if (!UserAuth.IsLoggedIn(Session))
            return false;
        if (UserAuth.IsAdmin(Session))
            return true;
        return UserAuth.UserId(Session) == itemUserId.ToString();
    }

    protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "DeleteItem")
            return;

        if (!UserAuth.IsLoggedIn(Session))
        {
            Response.Redirect("login.aspx?returnUrl=FoodBoard.aspx");
            return;
        }

        string itemId = e.CommandArgument.ToString();
        SupabaseRest.Delete("food_items", "id=eq." + itemId, UserAuth.AccessToken(Session));
        lblMessage.Text = "המוצר נמחק.";
        LoadItems(BuildSearchQuery());
    }
}
