using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

public static class SupabaseRest
{
    private static readonly HttpClient Http = new HttpClient();

    // query example: "select=*&city=eq.Tel Aviv&order=expiry_date.asc"
    public static List<Dictionary<string, object>> Select(string table, string query, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/" + table;
        if (!string.IsNullOrEmpty(query))
            url += "?" + query;

        using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        {
            AddAuthHeaders(request, userAccessToken);
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return new JavaScriptSerializer().Deserialize<List<Dictionary<string, object>>>(body);
        }
    }

    public static void Insert(string table, object row, string userAccessToken)
    {
        Send(HttpMethod.Post, table, "", row, userAccessToken);
    }

    public static void Update(string table, string query, object patch, string userAccessToken)
    {
        Send(new HttpMethod("PATCH"), table, query, patch, userAccessToken);
    }

    public static void Delete(string table, string query, string userAccessToken)
    {
        Send(HttpMethod.Delete, table, query, null, userAccessToken);
    }

    public static void Rpc(string functionName, object args, string userAccessToken)
    {
        RpcSelect(functionName, args, userAccessToken);
    }

    // for RPC functions that return a table/rows (e.g. "returns table (...)")
    public static List<Dictionary<string, object>> RpcSelect(string functionName, object args, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/rpc/" + functionName;
        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            AddAuthHeaders(request, userAccessToken);
            string json = new JavaScriptSerializer().Serialize(args ?? new Dictionary<string, object>());
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            if (string.IsNullOrEmpty(body))
                return new List<Dictionary<string, object>>();
            return new JavaScriptSerializer().Deserialize<List<Dictionary<string, object>>>(body);
        }
    }

    // for RPC functions that return a single scalar (e.g. "returns bigint"/"returns int")
    public static long RpcScalar(string functionName, object args, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/rpc/" + functionName;
        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            AddAuthHeaders(request, userAccessToken);
            string json = new JavaScriptSerializer().Serialize(args ?? new Dictionary<string, object>());
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            if (string.IsNullOrEmpty(body))
                return 0;
            return long.Parse(body);
        }
    }

    // for RPC functions that return a single boolean
    public static bool RpcBool(string functionName, object args, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/rpc/" + functionName;
        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            AddAuthHeaders(request, userAccessToken);
            string json = new JavaScriptSerializer().Serialize(args ?? new Dictionary<string, object>());
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return body.Trim() == "true";
        }
    }

    // for RPC functions that return a single nullable double (e.g. "returns double precision")
    public static double? RpcNullableDouble(string functionName, object args, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/rpc/" + functionName;
        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            AddAuthHeaders(request, userAccessToken);
            string json = new JavaScriptSerializer().Serialize(args ?? new Dictionary<string, object>());
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            body = body.Trim();
            if (string.IsNullOrEmpty(body) || body == "null")
                return null;
            return double.Parse(body, System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    private static void Send(HttpMethod method, string table, string query, object body, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/rest/v1/" + table;
        if (!string.IsNullOrEmpty(query))
            url += "?" + query;

        using (var request = new HttpRequestMessage(method, url))
        {
            AddAuthHeaders(request, userAccessToken);
            if (body != null)
            {
                string json = new JavaScriptSerializer().Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }
    }

    private static void AddAuthHeaders(HttpRequestMessage request, string userAccessToken)
    {
        request.Headers.Add("apikey", SupabaseConfig.AnonKey);
        request.Headers.Add("Authorization", "Bearer " + (string.IsNullOrEmpty(userAccessToken) ? SupabaseConfig.AnonKey : userAccessToken));
    }
}
