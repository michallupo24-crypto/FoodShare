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
    private class SearchQuery
    {
        public string Sql;
        public OleDbParameter[] Parameters;
    }

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
                LoadItems(BuildSearchSql());
            }
            else
            {
                LoadItems(null, null);
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
        LoadItems(null, null);
    }

    private SearchQuery BuildSearchSql()
    {
        string sql = @"SELECT FoodItems.ItemID, FoodItems.UserID, FoodItems.ItemName, FoodItems.Category,
                       FoodItems.PickupCity, FoodItems.PickupLocation, FoodItems.ExpiryDate, FoodItems.Quantity,
                       Users.UserName, DateDiff('d', Date(), FoodItems.ExpiryDate) AS DaysLeft
                       FROM FoodItems INNER JOIN Users ON FoodItems.UserID = Users.id
                       WHERE FoodItems.ExpiryDate >= Date()";

        var parameters = new System.Collections.Generic.List<OleDbParameter>();

        if (ddlCity.SelectedValue != "")
        {
            sql += " AND FoodItems.PickupCity = ?";
            parameters.Add(new OleDbParameter("PickupCity", OleDbType.VarWChar) { Value = ddlCity.SelectedValue });
        }

        if (ddlCategory.SelectedValue != "")
        {
            sql += " AND FoodItems.Category = ?";
            parameters.Add(new OleDbParameter("Category", OleDbType.VarWChar) { Value = ddlCategory.SelectedValue });
        }

        if (ddlExpiry.SelectedValue != "")
        {
            int days = Convert.ToInt32(ddlExpiry.SelectedValue);
            sql += " AND FoodItems.ExpiryDate <= DateAdd('d', ?, Date())";
            parameters.Add(new OleDbParameter("Days", OleDbType.Integer) { Value = days });
        }

        sql += " ORDER BY FoodItems.ExpiryDate ASC";
        return new SearchQuery { Sql = sql, Parameters = parameters.ToArray() };
    }

    private void LoadItems(SearchQuery query)
    {
        LoadItems(query.Sql, query.Parameters);
    }

    private void LoadItems(string sql, OleDbParameter[] parameters)
    {
        if (string.IsNullOrEmpty(sql))
        {
            sql = @"SELECT FoodItems.ItemID, FoodItems.UserID, FoodItems.ItemName, FoodItems.Category,
                    FoodItems.PickupCity, FoodItems.PickupLocation, FoodItems.ExpiryDate, FoodItems.Quantity,
                    Users.UserName, DateDiff('d', Date(), FoodItems.ExpiryDate) AS DaysLeft
                    FROM FoodItems INNER JOIN Users ON FoodItems.UserID = Users.id
                    WHERE FoodItems.ExpiryDate >= Date()
                    ORDER BY FoodItems.ExpiryDate ASC";
            parameters = new OleDbParameter[0];
        }

        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql, parameters);
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
        MyAdoHelperAccess.ExecuteNonQuery(
            "DELETE FROM FoodItems WHERE ItemID = ?",
            new OleDbParameter("ItemID", OleDbType.Integer) { Value = itemId });
        lblMessage.Text = "המוצר נמחק.";
        LoadItems(BuildSearchSql());
    }
}
