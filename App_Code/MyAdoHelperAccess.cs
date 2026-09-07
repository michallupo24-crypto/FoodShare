using System;
using System.Data;
using System.Data.OleDb;
using System.Web;

public class MyAdoHelperAccess
{
    public static string GetConnectionString()
    {
        string fileName = "FoodShare.accdb";
        string path = HttpContext.Current.Server.MapPath("~/App_Data/") + fileName;
        return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path;
    }

    public static bool IsExist(string sql)
    {
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }
    }

    public static DataTable ExecuteDataTable(string sql)
    {
        DataTable dt = new DataTable();
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
            da.Fill(dt);
        }
        return dt;
    }

    public static void ExecuteNonQuery(string sql)
    {
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
