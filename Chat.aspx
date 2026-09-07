<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="chat-container">
        <h2>שיחה עם <%= Server.HtmlEncode(OtherUsername) %></h2>
        <% if (!string.IsNullOrEmpty(ItemName)) { %>
            <p class="chat-item-context">בנוגע למוצר: <%= Server.HtmlEncode(ItemName) %></p>
        <% } %>

        <div id="chatHistory" class="chat-history">
            <asp:Literal ID="litHistory" runat="server"></asp:Literal>
        </div>

        <div class="chat-input-row">
            <input type="text" id="chatInput" placeholder="כתבו הודעה..." autocomplete="off" />
            <button type="button" id="chatSendBtn">שליחה</button>
        </div>
        <p id="chatError" class="chat-error"></p>
        <p><a href="Messages.aspx">חזרה להודעות</a></p>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/@supabase/supabase-js@2"></script>
    <script type="text/javascript">
        (function () {
            var supabaseUrl = <%= Sq(SupabaseConfig.Url) %>;
            var supabaseAnonKey = <%= Sq(SupabaseConfig.AnonKey) %>;
            var myId = <%= Sq(UserAuth.UserId(Session)) %>;
            var otherId = <%= Sq(OtherUserId) %>;
            var itemId = <%= string.IsNullOrEmpty(ItemId) ? "null" : Sq(ItemId) %>;
            var accessToken = <%= Sq(UserAuth.AccessToken(Session)) %>;
            var refreshToken = <%= Sq(UserAuth.RefreshToken(Session)) %>;

            var client = supabase.createClient(supabaseUrl, supabaseAnonKey);

            var historyEl = document.getElementById("chatHistory");
            var inputEl = document.getElementById("chatInput");
            var sendBtn = document.getElementById("chatSendBtn");
            var errorEl = document.getElementById("chatError");

            function escapeHtml(text) {
                var div = document.createElement("div");
                div.textContent = text;
                return div.innerHTML;
            }

            function appendBubble(body, mine) {
                var div = document.createElement("div");
                div.className = "chat-bubble " + (mine ? "mine" : "theirs");
                div.innerHTML = escapeHtml(body);
                historyEl.appendChild(div);
                historyEl.scrollTop = historyEl.scrollHeight;
            }

            function showError(msg) {
                errorEl.textContent = msg;
            }

            async function init() {
                var sessionResult = await client.auth.setSession({ access_token: accessToken, refresh_token: refreshToken });
                if (sessionResult.error) {
                    showError("החיבור לצ'אט החי נכשל - ההודעות עדיין נשלחות ונשמרות, אבל בלי עדכון מיידי. רעננו את הדף כדי לראות הודעות חדשות.");
                }

                client
                    .channel("chat-" + [myId, otherId].sort().join("-"))
                    .on("postgres_changes",
                        { event: "INSERT", schema: "public", table: "messages", filter: "receiver_id=eq." + myId },
                        function (payload) {
                            var m = payload.new;
                            if (m.sender_id === otherId) {
                                appendBubble(m.body, false);
                                client.rpc("mark_conversation_read", { other_user: otherId });
                            }
                        })
                    .subscribe();

                historyEl.scrollTop = historyEl.scrollHeight;
            }

            async function sendMessage() {
                var body = inputEl.value.trim();
                if (!body) return;

                sendBtn.disabled = true;
                var row = { sender_id: myId, receiver_id: otherId, body: body };
                if (itemId) row.item_id = itemId;

                var result = await client.from("messages").insert(row);
                sendBtn.disabled = false;

                if (result.error) {
                    showError("שליחת ההודעה נכשלה: " + result.error.message);
                    return;
                }

                showError("");
                appendBubble(body, true);
                inputEl.value = "";
                inputEl.focus();
            }

            sendBtn.addEventListener("click", sendMessage);
            inputEl.addEventListener("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    sendMessage();
                }
            });

            init();
        })();
    </script>
</asp:Content>
