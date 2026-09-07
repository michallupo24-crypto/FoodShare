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

        <% if (CanShareAddress) { %>
            <div class="chat-share-address-row">
                <button type="button" id="chatShareAddressBtn">&#128205; שלח/י כתובת מדויקת</button>
            </div>
        <% } %>

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
            var pickupLocation = <%= string.IsNullOrEmpty(ItemPickupLocation) ? "null" : Sq(ItemPickupLocation) %>;

            var client = supabase.createClient(supabaseUrl, supabaseAnonKey);

            var historyEl = document.getElementById("chatHistory");
            var inputEl = document.getElementById("chatInput");
            var sendBtn = document.getElementById("chatSendBtn");
            var shareAddressBtn = document.getElementById("chatShareAddressBtn");
            var errorEl = document.getElementById("chatError");

            function escapeHtml(text) {
                var div = document.createElement("div");
                div.textContent = text;
                return div.innerHTML;
            }

            function appendBubble(body, mine, messageType) {
                var div = document.createElement("div");
                div.className = "chat-bubble " + (mine ? "mine" : "theirs") + (messageType === "address" ? " address" : "");
                var html = "";
                if (messageType === "address")
                    html += "<strong>&#128205; כתובת לאיסוף:</strong><br/>";
                html += escapeHtml(body);
                div.innerHTML = html;
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
                                appendBubble(m.body, false, m.message_type);
                                client.rpc("mark_conversation_read", { other_user: otherId });
                            }
                        })
                    .subscribe();

                historyEl.scrollTop = historyEl.scrollHeight;
            }

            async function sendRow(body, messageType) {
                var row = { sender_id: myId, receiver_id: otherId, body: body, message_type: messageType };
                if (itemId) row.item_id = itemId;
                return await client.from("messages").insert(row);
            }

            async function sendMessage() {
                var body = inputEl.value.trim();
                if (!body) return;

                sendBtn.disabled = true;
                var result = await sendRow(body, "text");
                sendBtn.disabled = false;

                if (result.error) {
                    showError("שליחת ההודעה נכשלה: " + result.error.message);
                    return;
                }

                showError("");
                appendBubble(body, true, "text");
                inputEl.value = "";
                inputEl.focus();
            }

            async function shareAddress() {
                if (!pickupLocation) return;
                shareAddressBtn.disabled = true;
                var result = await sendRow(pickupLocation, "address");
                shareAddressBtn.disabled = false;

                if (result.error) {
                    showError("שליחת הכתובת נכשלה: " + result.error.message);
                    return;
                }

                showError("");
                appendBubble(pickupLocation, true, "address");
            }

            sendBtn.addEventListener("click", sendMessage);
            inputEl.addEventListener("keydown", function (e) {
                if (e.key === "Enter") {
                    e.preventDefault();
                    sendMessage();
                }
            });
            if (shareAddressBtn) {
                shareAddressBtn.addEventListener("click", shareAddress);
            }

            init();
        })();
    </script>
</asp:Content>
