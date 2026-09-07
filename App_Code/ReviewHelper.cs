using System;
using System.Collections.Generic;

public static class ReviewHelper
{
    public const int MinReviewsForPhoto = 2;

    public static int GetReviewCount(string userId, string token)
    {
        List<Dictionary<string, object>> summary = SupabaseRest.Select(
            "review_summaries", "reviewee_id=eq." + userId + "&select=review_count", token);

        if (summary.Count == 0)
            return 0;

        return Convert.ToInt32(summary[0]["review_count"]);
    }

    public static bool CanUploadPhoto(string userId, string token)
    {
        return GetReviewCount(userId, token) >= MinReviewsForPhoto;
    }
}
