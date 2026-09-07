using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

public partial class ProfilePage : System.Web.UI.Page
{
    protected string Username;
    protected string AverageRating = "0";
    protected int ReviewCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        string userId = Request.QueryString["id"];
        if (string.IsNullOrEmpty(userId))
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }

        string token = UserAuth.AccessToken(Session);

        List<Dictionary<string, object>> profile = SupabaseRest.Select(
            "profiles", "id=eq." + userId + "&select=username", token);

        if (profile.Count == 0)
        {
            Response.Redirect("FoodBoard.aspx");
            return;
        }
        Username = profile[0]["username"].ToString();

        try
        {
            List<Dictionary<string, object>> summary = SupabaseRest.Select(
                "review_summaries", "reviewee_id=eq." + userId + "&select=average_rating,review_count", token);

            if (summary.Count > 0)
            {
                AverageRating = summary[0]["average_rating"].ToString();
                ReviewCount = Convert.ToInt32(summary[0]["review_count"]);
            }

            List<Dictionary<string, object>> reviews = SupabaseRest.Select(
                "reviews",
                "reviewee_id=eq." + userId + "&select=rating,comment,created_at&order=created_at.desc",
                token);

            StringBuilder html = new StringBuilder();
            foreach (Dictionary<string, object> r in reviews)
            {
                int rating = Convert.ToInt32(r["rating"]);
                DateTime created = Convert.ToDateTime(r["created_at"]);
                string comment = r["comment"] != null ? r["comment"].ToString() : "";

                html.Append("<div class=\"review-card\">");
                html.Append("<span class=\"review-date\">" + created.ToString("dd/MM/yyyy") + "</span>");
                html.Append("<span class=\"review-rating\">" + new string('★', rating) + new string('☆', 5 - rating) + "</span>");
                if (!string.IsNullOrEmpty(comment))
                    html.Append("<p>" + Server.HtmlEncode(comment) + "</p>");
                html.Append("</div>");
            }
            litReviews.Text = html.ToString();
        }
        catch (Exception)
        {
            // טבלת reviews עדיין לא קיימת (לפני הרצת 004_reviews.sql)
            litReviews.Text = "<p>ביקורות עדיין לא זמינות.</p>";
        }
    }
}
