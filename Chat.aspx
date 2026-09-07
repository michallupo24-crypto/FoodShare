<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Chat.aspx.cs" Inherits="Chat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="chat-container">
        <h2>שיחה עם <a href='<%= "Profile.aspx?id=" + Server.UrlEncode(OtherUserId) %>'><%= Server.HtmlEncode(OtherUsername) %></a></h2>
        <% if (!string.IsNullOrEmpty(ItemName)) { %>
            <p class="chat-item-context">בנוגע למוצר: <%= Server.HtmlEncode(ItemName) %></p>
        <% } %>

        <div id="chatHistory" class="chat-history">
            <asp:Literal ID="litHistory" runat="server"></asp:Literal>
        </div>

        <% if (CanShareAddress) { %>
            <div class="chat-share-address-row">
                <button type="button" id="chatShareAddressBtn">&#128205; שלח/י כתובת מדויקת</button>
                <span class="chat-item-context">
                    <% if (!string.IsNullOrEmpty(ItemPickupLocation)) { %>
                        בלחיצה תישלח/יישלח לצד השני הכתובת שכתבת בפרסום המוצר.
                    <% } else { %>
                        בלחיצה יישלח לצד השני קישור למפה עם מיקום ה-GPS ששיתפת בפרסום המוצר.
                    <% } %>
                </span>
            </div>
        <% } %>

        <div class="chat-input-row">
            <input type="text" id="chatInput" placeholder="כתבו הודעה..." autocomplete="off" />
            <button type="button" id="chatSendBtn">שליחה</button>
        </div>
        <p id="chatError" class="chat-error"></p>

        <% if (CanCoordinatePickup) { %>
            <div class="pickup-coordination">
                <h3>תיאום שעת איסוף</h3>
                <p class="chat-item-context">סמנו טווחי שעות נוחים לכם בימים שנותרו עד שהמוצר פג תוקף (עד 14 יום קדימה). כשלשניכם יש טווח באותו יום, החפיפה ביניכם תסומן.</p>
                <div id="pickupDaysContainer"></div>
            </div>
        <% } %>

        <% if (CanReview && !AlreadyReviewed) { %>
            <div class="review-form">
                <h3>איך היה המפגש?</h3>
                <asp:Label ID="lblReviewMessage" runat="server"></asp:Label>
                <label>דירוג:</label>
                <asp:DropDownList ID="ddlRating" runat="server">
                    <asp:ListItem Value="5" Text="5 - מצוין" Selected="True" />
                    <asp:ListItem Value="4" Text="4 - טוב" />
                    <asp:ListItem Value="3" Text="3 - סביר" />
                    <asp:ListItem Value="2" Text="2 - לא טוב" />
                    <asp:ListItem Value="1" Text="1 - גרוע" />
                </asp:DropDownList>
                <br />
                <asp:TextBox ID="txtReviewComment" runat="server" TextMode="MultiLine" Rows="2" placeholder="הערה (לא חובה)..."></asp:TextBox>
                <br />
                <asp:Button ID="btnSubmitReview" runat="server" Text="שליחת ביקורת אנונימית" OnClick="btnSubmitReview_Click" />
            </div>
        <% } else if (AlreadyReviewed) { %>
            <p class="chat-item-context">כבר השארתם ביקורת על המפגש הזה. תודה!</p>
        <% } %>

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
            var itemLat = <%= ItemLat.HasValue ? ItemLat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null" %>;
            var itemLon = <%= ItemLon.HasValue ? ItemLon.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "null" %>;
            var itemExpiryDate = <%= string.IsNullOrEmpty(ItemExpiryDate) ? "null" : Sq(ItemExpiryDate) %>;
            var canCoordinatePickup = <%= CanCoordinatePickup ? "true" : "false" %>;

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
                if (messageType === "address") {
                    html += "<strong>&#128205; כתובת לאיסוף:</strong><br/>";
                    if (/^https?:\/\//.test(body)) {
                        html += "<a href=\"" + escapeHtml(body) + "\" target=\"_blank\" rel=\"noopener\">פתיחת המיקום במפה</a>";
                    } else {
                        html += escapeHtml(body);
                    }
                } else {
                    html += escapeHtml(body);
                }
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
                initPickupCoordination();
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
                var body = pickupLocation;
                if (!body && itemLat !== null && itemLon !== null) {
                    body = "https://www.openstreetmap.org/?mlat=" + itemLat + "&mlon=" + itemLon + "#map=17/" + itemLat + "/" + itemLon;
                }
                if (!body) {
                    showError("לא הוגדר מיקום איסוף למוצר הזה.");
                    return;
                }

                shareAddressBtn.disabled = true;
                var result = await sendRow(body, "address");
                shareAddressBtn.disabled = false;

                if (result.error) {
                    showError("שליחת הכתובת נכשלה: " + result.error.message);
                    return;
                }

                showError("");
                appendBubble(body, true, "address");
            }

            // ── תיאום שעת איסוף ──
            var pickupContainer = document.getElementById("pickupDaysContainer");
            var myPickupSlots = [];
            var theirPickupSlots = [];

            function buildDayList() {
                var days = [];
                if (!itemExpiryDate) return days;
                var today = new Date();
                today.setHours(0, 0, 0, 0);
                var expiry = new Date(itemExpiryDate + "T00:00:00");
                var maxDays = 14;
                for (var i = 0; i < maxDays; i++) {
                    var d = new Date(today);
                    d.setDate(d.getDate() + i);
                    if (d > expiry) break;
                    var iso = d.getFullYear() + "-" + String(d.getMonth() + 1).padStart(2, "0") + "-" + String(d.getDate()).padStart(2, "0");
                    var label = d.toLocaleDateString("he-IL", { weekday: "short", day: "numeric", month: "numeric" });
                    days.push({ iso: iso, label: label });
                }
                return days;
            }

            function toMinutes(t) {
                var parts = t.split(":");
                return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
            }

            function overlapRange(a, b) {
                var start = Math.max(toMinutes(a.start_time), toMinutes(b.start_time));
                var end = Math.min(toMinutes(a.end_time), toMinutes(b.end_time));
                if (start >= end) return null;
                var pad = function (n) { return String(Math.floor(n / 60)).padStart(2, "0") + ":" + String(n % 60).padStart(2, "0"); };
                return pad(start) + "–" + pad(end);
            }

            async function fetchPickupSlots() {
                var mineRes = await client.rpc("get_pickup_slots", { target_item: itemId, target_user: myId });
                var theirsRes = await client.rpc("get_pickup_slots", { target_item: itemId, target_user: otherId });
                myPickupSlots = mineRes.data || [];
                theirPickupSlots = theirsRes.data || [];
            }

            async function addPickupSlot(dateIso, startT, endT) {
                var result = await client.rpc("add_pickup_slot", { target_item: itemId, target_date: dateIso, start_t: startT, end_t: endT });
                if (result.error) {
                    showError("הוספת הטווח נכשלה: " + result.error.message);
                    return false;
                }
                showError("");
                return true;
            }

            async function removePickupSlot(id) {
                await client.rpc("remove_pickup_slot", { target_id: id });
            }

            function renderPickupDays() {
                var days = buildDayList();
                pickupContainer.innerHTML = "";

                days.forEach(function (day) {
                    var mine = myPickupSlots.filter(function (s) { return s.slot_date === day.iso; });
                    var theirs = theirPickupSlots.filter(function (s) { return s.slot_date === day.iso; });

                    var dayEl = document.createElement("div");
                    dayEl.className = "pickup-day";

                    var title = document.createElement("div");
                    title.className = "pickup-day-title";
                    title.textContent = day.label;
                    dayEl.appendChild(title);

                    mine.forEach(function (s) {
                        var chip = document.createElement("span");
                        chip.className = "pickup-chip mine";
                        chip.textContent = s.start_time.slice(0, 5) + "–" + s.end_time.slice(0, 5) + " ✕";
                        chip.title = "לחצו להסרה";
                        chip.addEventListener("click", async function () {
                            await removePickupSlot(s.id);
                            await fetchPickupSlots();
                            renderPickupDays();
                        });
                        dayEl.appendChild(chip);
                    });

                    theirs.forEach(function (t) {
                        var overlapMine = mine.filter(function (s) { return overlapRange(s, t) !== null; });
                        if (overlapMine.length > 0) {
                            overlapMine.forEach(function (s) {
                                var chip = document.createElement("span");
                                chip.className = "pickup-chip overlap";
                                chip.textContent = "✓ חפיפה " + overlapRange(s, t);
                                dayEl.appendChild(chip);
                            });
                        } else {
                            var chip = document.createElement("span");
                            chip.className = "pickup-chip suggestion";
                            chip.textContent = "מוצע ע\"י הצד השני: " + t.start_time.slice(0, 5) + "–" + t.end_time.slice(0, 5);
                            var addBtn = document.createElement("button");
                            addBtn.type = "button";
                            addBtn.textContent = "גם אני פנוי/ה";
                            addBtn.addEventListener("click", async function () {
                                var ok = await addPickupSlot(day.iso, t.start_time.slice(0, 5), t.end_time.slice(0, 5));
                                if (ok) {
                                    await fetchPickupSlots();
                                    renderPickupDays();
                                }
                            });
                            chip.appendChild(addBtn);
                            dayEl.appendChild(chip);
                        }
                    });

                    var form = document.createElement("div");
                    form.className = "pickup-add-form";
                    var startInput = document.createElement("input");
                    startInput.type = "time";
                    var endInput = document.createElement("input");
                    endInput.type = "time";
                    var addRangeBtn = document.createElement("button");
                    addRangeBtn.type = "button";
                    addRangeBtn.textContent = "הוסיפו טווח";
                    addRangeBtn.addEventListener("click", async function () {
                        if (!startInput.value || !endInput.value) return;
                        var ok = await addPickupSlot(day.iso, startInput.value, endInput.value);
                        if (ok) {
                            await fetchPickupSlots();
                            renderPickupDays();
                        }
                    });
                    form.appendChild(startInput);
                    form.appendChild(endInput);
                    form.appendChild(addRangeBtn);
                    dayEl.appendChild(form);

                    pickupContainer.appendChild(dayEl);
                });
            }

            async function initPickupCoordination() {
                if (!canCoordinatePickup || !pickupContainer) return;
                await fetchPickupSlots();
                renderPickupDays();
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
