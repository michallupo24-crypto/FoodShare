using System;
using System.Data;
using System.Data.OleDb;
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
            LoadItems(null);
        }
        if (!IsPostBack)
        {
            LoadCityFilter();
            LoadCategoryFilter();

            // בדיקה אם הגיע סינון מהלינק
            string catFromUrl = Request.QueryString["cat"];
            if (!string.IsNullOrEmpty(catFromUrl))
            {
                ddlCategory.SelectedValue = catFromUrl;
                LoadItems(BuildSearchSql()); // טעינה עם סינון
            }
            else
            {
                LoadItems(null);
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
        LoadItems(BuildSearchSql());
        lblMessage.Text = "הסינון בוצע.";
    }

    protected void btnShowAll_Click(object sender, EventArgs e)
    {
        ddlCity.SelectedIndex = 0;
        ddlCategory.SelectedIndex = 0;
        ddlExpiry.SelectedIndex = 0;
        lblMessage.Text = "";
        LoadItems(null);
    }

    private string BuildSearchSql()
    {
        string sql = @"SELECT FoodItems.ItemID, FoodItems.UserID, FoodItems.ItemName, FoodItems.Category,
                       FoodItems.PickupCity, FoodItems.PickupLocation, FoodItems.ExpiryDate, FoodItems.Quantity,
                       Users.UserName, DateDiff('d', Date(), FoodItems.ExpiryDate) AS DaysLeft
                       FROM FoodItems INNER JOIN Users ON FoodItems.UserID = Users.id
                       WHERE FoodItems.ExpiryDate >= Date()";

        if (ddlCity.SelectedValue != "")
            sql += " AND FoodItems.PickupCity = '" + ddlCity.SelectedValue.Replace("'", "''") + "'";

        if (ddlCategory.SelectedValue != "")
            sql += " AND FoodItems.Category = '" + ddlCategory.SelectedValue.Replace("'", "''") + "'";

        if (ddlExpiry.SelectedValue != "")
        {
            int days = Convert.ToInt32(ddlExpiry.SelectedValue);
            sql += " AND FoodItems.ExpiryDate <= DateAdd('d', " + days + ", Date())";
        }

        sql += " ORDER BY FoodItems.ExpiryDate ASC";
        return sql;
    }

    private void LoadItems(string sql)
    {
        if (string.IsNullOrEmpty(sql))
        {
            sql = @"SELECT FoodItems.ItemID, FoodItems.UserID, FoodItems.ItemName, FoodItems.Category,
                    FoodItems.PickupCity, FoodItems.PickupLocation, FoodItems.ExpiryDate, FoodItems.Quantity,
                    Users.UserName, DateDiff('d', Date(), FoodItems.ExpiryDate) AS DaysLeft
                    FROM FoodItems INNER JOIN Users ON FoodItems.UserID = Users.id
                    WHERE FoodItems.ExpiryDate >= Date()
                    ORDER BY FoodItems.ExpiryDate ASC";
        }

        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql);
        gvItems.DataSource = dt;
        gvItems.DataBind();
    }

    public bool CanEdit(object itemUserId)
    {
        if (!UserAuth.IsLoggedIn(Session))
            return false;
        if (UserAuth.IsAdmin(Session))
            return true;
        return Session["UserID"].ToString() == itemUserId.ToString();
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

        int itemId = Convert.ToInt32(e.CommandArgument);
        MyAdoHelperAccess.ExecuteNonQuery("DELETE FROM FoodItems WHERE ItemID = " + itemId);
        lblMessage.Text = "המוצר נמחק.";
        LoadItems(BuildSearchSql());
    }
}
