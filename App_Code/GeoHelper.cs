using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class GeoHelper
{
    // מרכז העיר בלבד - מספיק בשביל הערכת מרחק גסה, בלי לחשוף כתובת מדויקת
    private static readonly Dictionary<string, double[]> CityCoords = new Dictionary<string, double[]>
    {
        { "Tel Aviv", new double[] { 32.0853, 34.7818 } },
        { "Jerusalem", new double[] { 31.7683, 35.2137 } },
        { "Haifa", new double[] { 32.7940, 34.9896 } },
        { "Beersheba", new double[] { 31.2530, 34.7915 } }
    };

    // מרחק משוער בק"מ בין שתי ערים (קו ישר ממרכז לעיר) - null אם אחת הערים לא מוכרת
    public static double? DistanceKm(string cityA, string cityB)
    {
        if (string.IsNullOrEmpty(cityA) || string.IsNullOrEmpty(cityB))
            return null;

        double[] a, b;
        if (!CityCoords.TryGetValue(cityA, out a) || !CityCoords.TryGetValue(cityB, out b))
            return null;

        if (cityA == cityB)
            return 0;

        return Haversine(a[0], a[1], b[0], b[1]);
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371;
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double h = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(h), Math.Sqrt(1 - h));
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    // מסתירה את מספר הבית/הפרטים המדויקים, משאירה רק את שם הרחוב/האזור הכללי
    public static string PartialLocation(string fullLocation)
    {
        if (string.IsNullOrWhiteSpace(fullLocation))
            return fullLocation;

        string trimmed = Regex.Replace(fullLocation.Trim(), @"\s*\d+\s*$", "");
        return string.IsNullOrWhiteSpace(trimmed) ? "(פרטים מלאים בצ'אט עם המפרסם/ת)" : trimmed;
    }
}
