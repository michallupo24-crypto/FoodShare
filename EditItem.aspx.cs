using System;
using System.Data;
using System.Data.OleDb;
using System.Web.UI;

public partial class EditItem : System.Web.UI.Page
{
    private int itemId;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, Request.RawUrl);

        if (!int.TryParse(Request.QueryString["id"], out itemId))
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        if (!IsPostBack)
            LoadItem();
    }

    private void LoadItem()
    {
        string sql = "SELECT * FROM FoodItems WHERE ItemID = " + itemId;
        DataTable dt = MyAdoHelperAccess.ExecuteDataTable(sql);
        if (dt.Rows.Count == 0)
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        DataRow r = dt.Rows[0];
        if (!CanEdit(r["UserID"].ToString()))
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        txtItemName.Text = r["ItemName"].ToString();
        txtExpiry.Text = Convert.ToDateTime(r["ExpiryDate"]).ToString("yyyy-MM-dd");
        txtQuantity.Text = r["Quantity"].ToString();
        txtCategory.Text = r["Category"].ToString();
        txtLocation.Text = r["PickupLocation"].ToString();

        if (r["PickupCity"] != DBNull.Value && ddlCity.Items.FindByValue(r["PickupCity"].ToString()) != null)
            ddlCity.SelectedValue = r["PickupCity"].ToString();
    }

    private bool CanEdit(string ownerId)
    {
        if (UserAuth.IsAdmin(Session))
            return true;
        return Session["UserID"].ToString() == ownerId;
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        string sql = @"UPDATE FoodItems SET ItemName=?, ExpiryDate=?, Quantity=?, Category=?, PickupCity=?, PickupLocation=?
                       WHERE ItemID=?";

        using (OleDbConnection conn = new OleDbConnection(MyAdoHelperAccess.GetConnectionString()))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(new OleDbParameter("ItemName", OleDbType.VarWChar) { Value = txtItemName.Text.Trim() });
            cmd.Parameters.Add(new OleDbParameter("ExpiryDate", OleDbType.Date) { Value = DateTime.Parse(txtExpiry.Text) });
            cmd.Parameters.Add(new OleDbParameter("Quantity", OleDbType.Integer) { Value = Convert.ToInt32(txtQuantity.Text) });
            cmd.Parameters.Add(new OleDbParameter("Category", OleDbType.VarWChar) { Value = txtCategory.Text.Trim() });
            cmd.Parameters.Add(new OleDbParameter("PickupCity", OleDbType.VarWChar) { Value = ddlCity.SelectedValue });
            cmd.Parameters.Add(new OleDbParameter("PickupLocation", OleDbType.VarWChar) { Value = txtLocation.Text.Trim() });
            cmd.Parameters.Add(new OleDbParameter("ItemID", OleDbType.Integer) { Value = itemId });
            cmd.ExecuteNonQuery();
        }

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המוצר עודכן בהצלחה.";
    }
}
