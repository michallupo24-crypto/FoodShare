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

        <% if (CanUploadPhoto) { %>
            <label>תמונה של המוצר (לא חובה):</label><br />
            <asp:FileUpload ID="fuPhoto" runat="server" /><br /><br />
        <% } else { %>
            <p class="chat-item-context">ניתן להוסיף תמונה למוצרים רק אחרי שקיבלתם לפחות 2 ביקורות (כרגע: <%= ReviewCount %>).</p>
        <% } %>

        <asp:Button ID="btnAdd" runat="server" Text="פרסם מוצר" OnClick="btnAdd_Click" />
    </div>
</asp:Content>
