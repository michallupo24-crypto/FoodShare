<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AddItem.aspx.cs" Inherits="AddItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>הוספת מוצר לשיתוף</h2>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label><br />

        <label>שם המוצר:</label><br />
        <asp:TextBox ID="txtItemName" runat="server" required="true"></asp:TextBox><br />

        <label>תאריך תפוגה:</label><br />
        <asp:TextBox ID="txtExpiry" runat="server" TextMode="Date" required="true"></asp:TextBox><br />

        <label>כמות:</label><br />
        <asp:TextBox ID="txtQuantity" runat="server" required="true"></asp:TextBox><br />

        <label>קטגוריה:</label><br />
        <asp:DropDownList ID="ddlCategory" runat="server"></asp:DropDownList><br />

        <label>עיר איסוף:</label><br />
        <asp:DropDownList ID="ddlCity" runat="server">
            <asp:ListItem Value="Tel Aviv">תל אביב</asp:ListItem>
            <asp:ListItem Value="Jerusalem">ירושלים</asp:ListItem>
            <asp:ListItem Value="Haifa">חיפה</asp:ListItem>
            <asp:ListItem Value="Beersheba">באר שבע</asp:ListItem>
        </asp:DropDownList><br />

        <label>כתובת / מיקום מדויק:</label><br />
        <asp:TextBox ID="txtLocation" runat="server" placeholder="רחוב, שכונה..." required="true"></asp:TextBox><br /><br />

        <div class='<%= "photo-gate" + (CanUploadPhoto ? " unlocked" : "") %>'>
            <div class="photo-gate-title"><%= CanUploadPhoto ? "תמונות נפתחו" : "תמונות ייפתחו אחרי 2 ביקורות" %></div>
            <div class="photo-gate-body">
                <% if (CanUploadPhoto) { %>
                    אפשר לצרף תמונה אחת למוצר. תמונה שדווחה מוסתרת עד שמנהל/ת בודק/ת אותה.
                <% } else { %>
                    אחרי שתי מסירות עם ביקורת נפתחת האפשרות לצרף תמונה למוצר.
                <% } %>
            </div>
            <div class="photo-gate-pips">
                <div class='<%= "pip" + (ReviewCount >= 1 ? " filled" : "") %>'></div>
                <div class='<%= "pip" + (ReviewCount >= 2 ? " filled" : "") %>'></div>
                <span><%= Math.Min(ReviewCount, 2) %> מתוך 2 ביקורות</span>
            </div>
            <% if (CanUploadPhoto) { %>
                <br />
                <asp:FileUpload ID="fuPhoto" runat="server" />
            <% } %>
        </div>

        <asp:Button ID="btnAdd" runat="server" Text="פרסם מוצר" OnClick="btnAdd_Click" />
    </div>
</asp:Content>
