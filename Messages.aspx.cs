using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

public partial class MessagesPage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        UserAuth.RequireLogin(Session, "Messages.aspx");

        if (!IsPostBack)
            LoadConversations();
    }

    private void LoadConversations()
    {
        List<Dictionary<string, object>> conversations = SupabaseRest.RpcSelect(
            "list_conversations", null, UserAuth.AccessToken(Session));

        if (conversations.Count == 0)
        {
            litConversations.Text = "<p>עדיין אין לך הודעות. אפשר להתחיל שיחה מתוך מודעה בלוח.</p>";
            return;
        }

        StringBuilder html = new StringBuilder();
        html.Append("<div class=\"conversation-list\">");

        foreach (Dictionary<string, object> c in conversations)
        {
            string otherId = c["other_user_id"].ToString();
            string otherUsername = c["other_username"].ToString();
            string lastMessage = c["last_message"].ToString();
            bool hasUnread = Convert.ToInt32(c["unread_count"]) > 0;

            html.Append("<a class=\"conversation-row" + (hasUnread ? " unread" : "") + "\" href=\"Chat.aspx?with=" + Server.UrlEncode(otherId) + "\">");
            html.Append("<span class=\"conversation-name\">" + Server.HtmlEncode(otherUsername) + "</span>");
            html.Append("<span class=\"conversation-preview\">" + Server.HtmlEncode(lastMessage) + "</span>");
            if (hasUnread)
                html.Append("<span class=\"conversation-badge\">" + c["unread_count"] + "</span>");
            html.Append("</a>");
        }

        html.Append("</div>");
        litConversations.Text = html.ToString();
    }
}
