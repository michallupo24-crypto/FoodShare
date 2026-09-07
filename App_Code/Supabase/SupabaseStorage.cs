using System.Net.Http;
using System.Net.Http.Headers;

public static class SupabaseStorage
{
    private static readonly HttpClient Http = new HttpClient();

    // path should start with the uploader's own user id (e.g. "<userId>/<guid>-photo.jpg")
    // to satisfy the storage RLS policy that only allows uploads into your own folder.
    public static string Upload(string bucket, string path, byte[] fileBytes, string contentType, string userAccessToken)
    {
        string url = SupabaseConfig.Url + "/storage/v1/object/" + bucket + "/" + path;
        using (var request = new HttpRequestMessage(HttpMethod.Post, url))
        {
            request.Headers.Add("apikey", SupabaseConfig.AnonKey);
            request.Headers.Add("Authorization", "Bearer " + userAccessToken);
            request.Content = new ByteArrayContent(fileBytes);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType);

            HttpResponseMessage response = Http.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        return SupabaseConfig.Url + "/storage/v1/object/public/" + bucket + "/" + path;
    }
}
