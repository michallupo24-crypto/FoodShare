using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Web.UI;

public partial class MapPage : System.Web.UI.Page
{
    protected string CitiesJson;

    protected void Page_Load(object sender, EventArgs e)
    {
        string token = UserAuth.AccessToken(Session);
        string viewerCity = UserAuth.IsLoggedIn(Session) ? UserAuth.UserCity(Session) : null;

        Dictionary<string, int> counts = new Dictionary<string, int>();
        try
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            List<Dictionary<string, object>> rows = SupabaseRest.Select(
                "food_items", "select=pickup_city&expiry_date=gte." + today, token);

            foreach (Dictionary<string, object> row in rows)
            {
                string city = row["pickup_city"].ToString();
                counts[city] = counts.ContainsKey(city) ? counts[city] + 1 : 1;
            }
        }
        catch (Exception)
        {
            // אם השאילתה נכשלת מסיבה כלשהי, פשוט מציגים את המפה עם 0 בכל מקום
        }

        List<Dictionary<string, object>> cities = new List<Dictionary<string, object>>();
        foreach (GeoHelper.CityInfo city in GeoHelper.GetAllCities())
        {
            int count = counts.ContainsKey(city.EnglishName) ? counts[city.EnglishName] : 0;
            double? distance = GeoHelper.DistanceKm(viewerCity, city.EnglishName);

            cities.Add(new Dictionary<string, object>
            {
                { "en", city.EnglishName },
                { "he", city.HebrewName },
                { "lat", city.Lat },
                { "lon", city.Lon },
                { "count", count },
                { "distance", distance.HasValue ? Math.Round(distance.Value).ToString() : null }
            });
        }

        CitiesJson = new JavaScriptSerializer().Serialize(cities);
    }
}
