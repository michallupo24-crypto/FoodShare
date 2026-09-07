using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

public class SupabaseAuthResult
{
    public bool Success;
    public string ErrorMessage;
    public string UserId;
    public string AccessToken;
    public string RefreshToken;
}

public static class SupabaseAuth
{
    private static readonly HttpClient Http = new HttpClient();

    public static SupabaseAuthResult SignUp(string email, string password)
    {
        return PostAuth("signup", email, password);
    }

    public static SupabaseAuthResult SignIn(string email, string password)
    {
        return PostAuth("token?grant_type=password", email, password);
    }

    private static SupabaseAuthResult PostAuth(string path, string email, string password)
    {
        var serializer = new JavaScriptSerializer();
        string body = serializer.Serialize(new Dictionary<string, object> { { "email", email }, { "password", password } });

        using (var request = new HttpRequestMessage(HttpMethod.Post, SupabaseConfig.Url + "/auth/v1/" + path))
        {
            request.Headers.Add("apikey", SupabaseConfig.AnonKey);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Dictionary<string, object> data;
            try
            {
                data = serializer.Deserialize<Dictionary<string, object>>(responseBody);
            }
            catch
            {
                return new SupabaseAuthResult { Success = false, ErrorMessage = "תגובה לא תקינה מהשרת." };
            }

            if (!response.IsSuccessStatusCode || data == null)
            {
                string message = "שגיאה מהשרת.";
                if (data != null)
                {
                    if (data.ContainsKey("msg"))
                        message = data["msg"].ToString();
                    else if (data.ContainsKey("error_description"))
                        message = data["error_description"].ToString();
                    else if (data.ContainsKey("message"))
                        message = data["message"].ToString();
                }
                return new SupabaseAuthResult { Success = false, ErrorMessage = message };
            }

            if (!data.ContainsKey("access_token"))
                return new SupabaseAuthResult { Success = false, ErrorMessage = "נדרש אימות מייל לפני ההתחברות הראשונה (בדקו את תיבת הדואר)." };

            var userObj = (Dictionary<string, object>)data["user"];

            return new SupabaseAuthResult
            {
                Success = true,
                UserId = userObj["id"].ToString(),
                AccessToken = data["access_token"].ToString(),
                RefreshToken = data["refresh_token"].ToString()
            };
        }
    }
}
