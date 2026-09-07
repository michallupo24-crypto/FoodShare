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

    public static bool IsExist(string sql, params OleDbParameter[] parameters)
    {
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            conn.Open();
            object result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }
    }

    public static DataTable ExecuteDataTable(string sql, params OleDbParameter[] parameters)
    {
        DataTable dt = new DataTable();
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
        }
        return dt;
    }

    public static void ExecuteNonQuery(string sql, params OleDbParameter[] parameters)
    {
        using (OleDbConnection conn = new OleDbConnection(GetConnectionString()))
        {
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddRange(parameters);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
