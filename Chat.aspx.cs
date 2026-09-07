using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

public partial class Chat : System.Web.UI.Page
{
    protected string OtherUserId;
    protected string OtherUsername;
    protected string ItemId;
    protected string ItemName;

    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, Request.RawUrl);

        OtherUserId = Request.QueryString["with"];
        if (string.IsNullOrEmpty(OtherUserId))
        {
            Response.Redirect("Messages.aspx");
            return;
        }

        ItemId = Request.QueryString["item"];
        string token = UserAuth.AccessToken(Session);

        List<Dictionary<string, object>> otherProfile = SupabaseRest.Select(
            "profiles", "id=eq." + OtherUserId + "&select=username", token);

        if (otherProfile.Count == 0)
        {
            Response.Redirect("Messages.aspx");
            return;
        }
        OtherUsername = otherProfile[0]["username"].ToString();

        if (!string.IsNullOrEmpty(ItemId))
        {
            List<Dictionary<string, object>> item = SupabaseRest.Select(
                "food_items", "id=eq." + ItemId + "&select=item_name", token);
            if (item.Count > 0)
                ItemName = item[0]["item_name"].ToString();
        }

        if (!IsPostBack)
        {
            LoadHistory(token);
            SupabaseRest.Rpc("mark_conversation_read",
                new Dictionary<string, object> { { "other_user", OtherUserId } }, token);
        }
    }

    private void LoadHistory(string token)
    {
        string myId = UserAuth.UserId(Session);
        string query = "select=*&or=(and(sender_id.eq." + myId + ",receiver_id.eq." + OtherUserId + "),and(sender_id.eq." + OtherUserId + ",receiver_id.eq." + myId + "))&order=created_at.asc";

        List<Dictionary<string, object>> messages = SupabaseRest.Select("messages", query, token);

        StringBuilder html = new StringBuilder();
        foreach (Dictionary<string, object> m in messages)
        {
            bool mine = m["sender_id"].ToString() == myId;
            html.Append("<div class=\"chat-bubble " + (mine ? "mine" : "theirs") + "\" data-id=\"" + m["id"] + "\">");
            html.Append(Server.HtmlEncode(m["body"].ToString()));
            html.Append("</div>");
        }
        litHistory.Text = html.ToString();
    }

    protected string Sq(string value)
    {
        return "\"" + HttpUtility.JavaScriptStringEncode(value) + "\"";
    }
}
