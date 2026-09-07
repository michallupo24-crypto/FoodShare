<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FoodBoard.aspx.cs" Inherits="FoodBoard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>לוח שיתוף המזון</h2>
    <p>סינון לפי עיר איסוף, קטגוריה וזמן עד פג תוקף (אפשר לבחור אחד או יותר):</p>
    <p class="chat-item-context">מטעמי פרטיות מוצגת כאן רק כתובת חלקית ומרחק משוער. הכתובת המדויקת נמסרת ע"י המפרסם/ת דרך הצ'אט.</p>

    <div class="form-container">
        <label>עיר איסוף:</label>
        <asp:DropDownList ID="ddlCity" runat="server"></asp:DropDownList><br />

        <label>קטגוריה:</label>
        <asp:DropDownList ID="ddlCategory" runat="server"></asp:DropDownList><br />

        <label>פג תוקף תוך:</label>
        <asp:DropDownList ID="ddlExpiry" runat="server">
            <asp:ListItem Value="" Text="הכל" Selected="True" />
            <asp:ListItem Value="3" Text="3 ימים" />
            <asp:ListItem Value="7" Text="7 ימים" />
            <asp:ListItem Value="14" Text="14 יום" />
            <asp:ListItem Value="30" Text="30 יום" />
        </asp:DropDownList><br /><br />

        <asp:Button ID="btnSearch" runat="server" Text="סנן" OnClick="btnSearch_Click" />
        <asp:Button ID="btnShowAll" runat="server" Text="הצג הכל" OnClick="btnShowAll_Click" />
    </div>

    <asp:Label ID="lblMessage" runat="server"></asp:Label><br />

    <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" CssClass="grid-table"
        DataKeyNames="ItemID" OnRowCommand="gvItems_RowCommand" EmptyDataText="לא נמצאו מוצרים.">
        <Columns>
            <asp:TemplateField HeaderText="תמונה">
                <ItemTemplate>
                    <asp:Image ID="imgPhoto" runat="server" ImageUrl='<%# Eval("PhotoUrl") %>' CssClass="item-photo-thumb" Visible='<%# Eval("PhotoUrl").ToString() != "" %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ItemName" HeaderText="שם המוצר" />
            <asp:BoundField DataField="Category" HeaderText="קטגוריה" />
            <asp:BoundField DataField="PickupCity" HeaderText="עיר איסוף" />
            <asp:BoundField DataField="PickupLocation" HeaderText="אזור כללי" />
            <asp:BoundField DataField="Distance" HeaderText="מרחק ממך" />
            <asp:BoundField DataField="ExpiryDate" HeaderText="תאריך תפוגה" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="DaysLeft" HeaderText="ימים לפג תוקף" />
            <asp:BoundField DataField="Quantity" HeaderText="כמות" />
            <asp:TemplateField HeaderText="מפרסם">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkProfile" runat="server" NavigateUrl='<%# "Profile.aspx?id=" + Eval("UserID") %>'
                        Text='<%# Eval("UserName") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="פעולות">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl='<%# "EditItem.aspx?id=" + Eval("ItemID") %>'
                        Text="עריכה" Visible='<%# CanEdit(Eval("UserID")) %>' />
                    <asp:HyperLink ID="lnkMessage" runat="server" NavigateUrl='<%# "Chat.aspx?with=" + Eval("UserID") + "&item=" + Eval("ItemID") %>'
                        Text="שלח הודעה" Visible='<%# CanMessage(Eval("UserID")) %>' />
                    <asp:Button ID="btnDelete" runat="server" Text="מחיקה" CommandName="DeleteItem"
                        CommandArgument='<%# Eval("ItemID") %>'
                        Visible='<%# CanEdit(Eval("UserID")) %>'
                        OnClientClick="return confirm('למחוק את המוצר?');" />
                    <asp:Button ID="btnReportPhoto" runat="server" Text="דווח על תמונה" CommandName="ReportPhoto"
                        CommandArgument='<%# Eval("ItemID") %>'
                        Visible='<%# CanReportPhoto(Eval("UserID"), Eval("PhotoUrl")) %>'
                        OnClientClick="return confirm('לדווח על התמונה כלא הולמת/מזויפת?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
