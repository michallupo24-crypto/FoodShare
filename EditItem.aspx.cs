using System;
using System.Collections.Generic;
using System.Web.UI;

public partial class EditItem : System.Web.UI.Page
{
    private string itemId;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, Request.RawUrl);

        itemId = Request.QueryString["id"];
        if (string.IsNullOrEmpty(itemId))
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        if (!IsPostBack)
            LoadItem();
    }

    private void LoadItem()
    {
        List<Dictionary<string, object>> rows = SupabaseRest.Select(
            "food_items", "id=eq." + itemId + "&select=id,user_id,item_name,expiry_date,quantity,category,pickup_city,pickup_location",
            UserAuth.AccessToken(Session));

        if (rows.Count == 0)
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        Dictionary<string, object> r = rows[0];
        if (!CanEdit(r["user_id"].ToString()))
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        txtItemName.Text = r["item_name"].ToString();
        txtExpiry.Text = Convert.ToDateTime(r["expiry_date"]).ToString("yyyy-MM-dd");
        txtQuantity.Text = r["quantity"].ToString();
        txtCategory.Text = r["category"].ToString();
        txtLocation.Text = r["pickup_location"].ToString();

        string pickupCity = r["pickup_city"] != null ? r["pickup_city"].ToString() : "";
        if (pickupCity != "" && ddlCity.Items.FindByValue(pickupCity) != null)
            ddlCity.SelectedValue = pickupCity;
    }

    private bool CanEdit(string ownerId)
    {
        if (UserAuth.IsAdmin(Session))
            return true;
        return UserAuth.UserId(Session) == ownerId;
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        var patch = new Dictionary<string, object>
        {
            { "item_name", txtItemName.Text.Trim() },
            { "expiry_date", DateTime.Parse(txtExpiry.Text).ToString("yyyy-MM-dd") },
            { "quantity", Convert.ToInt32(txtQuantity.Text) },
            { "category", txtCategory.Text.Trim() },
            { "pickup_city", ddlCity.SelectedValue },
            { "pickup_location", txtLocation.Text.Trim() }
        };

        SupabaseRest.Update("food_items", "id=eq." + itemId, patch, UserAuth.AccessToken(Session));

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המוצר עודכן בהצלחה.";
    }
}
