using System;
using System.Data.OleDb;
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
        if (Session["UserID"] == null)
        {
            lblMessage.Text = "יש להתחבר לפני הוספת מוצר.";
            return;
        }

        string sql = @"INSERT INTO FoodItems (UserID, ItemName, ExpiryDate, Quantity, Category, PickupCity, PickupLocation, PostDate)
                       VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

        using (OleDbConnection conn = new OleDbConnection(MyAdoHelperAccess.GetConnectionString()))
        {
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.Add(new OleDbParameter("UserID", OleDbType.Integer) { Value = Convert.ToInt32(Session["UserID"]) });
            cmd.Parameters.Add(new OleDbParameter("ItemName", OleDbType.VarWChar) { Value = txtItemName.Text.Trim() });
            cmd.Parameters.Add(new OleDbParameter("ExpiryDate", OleDbType.Date) { Value = DateTime.Parse(txtExpiry.Text) });
            cmd.Parameters.Add(new OleDbParameter("Quantity", OleDbType.Integer) { Value = Convert.ToInt32(txtQuantity.Text) });
            cmd.Parameters.Add(new OleDbParameter("Category", OleDbType.VarWChar) { Value = ddlCategory.SelectedValue });
            cmd.Parameters.Add(new OleDbParameter("PickupCity", OleDbType.VarWChar) { Value = ddlCity.SelectedValue });
            cmd.Parameters.Add(new OleDbParameter("PickupLocation", OleDbType.VarWChar) { Value = txtLocation.Text.Trim() });
            cmd.Parameters.Add(new OleDbParameter("PostDate", OleDbType.Date) { Value = DateTime.Now });

            cmd.ExecuteNonQuery();
        }

        lblMessage.ForeColor = System.Drawing.Color.Green;
        lblMessage.Text = "המוצר פורסם בהצלחה!";
        txtItemName.Text = "";
        txtQuantity.Text = "";
        txtLocation.Text = "";
    }
}
