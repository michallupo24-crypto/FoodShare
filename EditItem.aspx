<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditItem.aspx.cs" Inherits="EditItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>עדכון מוצר</h2>
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label><br />

        <label>שם המוצר:</label><br />
        <asp:TextBox ID="txtItemName" runat="server"></asp:TextBox><br />

        <label>תאריך תפוגה:</label><br />
        <asp:TextBox ID="txtExpiry" runat="server" TextMode="Date"></asp:TextBox><br />

        <label>כמות:</label><br />
        <asp:TextBox ID="txtQuantity" runat="server"></asp:TextBox><br />

        <label>קטגוריה:</label><br />
        <asp:TextBox ID="txtCategory" runat="server"></asp:TextBox><br />

        <label>עיר איסוף:</label><br />
        <asp:DropDownList ID="ddlCity" runat="server">
            <asp:ListItem Value="Tel Aviv">תל אביב</asp:ListItem>
            <asp:ListItem Value="Jerusalem">ירושלים</asp:ListItem>
            <asp:ListItem Value="Haifa">חיפה</asp:ListItem>
            <asp:ListItem Value="Beersheba">באר שבע</asp:ListItem>
        </asp:DropDownList><br />

        <label>כתובת / מיקום:</label><br />
        <asp:TextBox ID="txtLocation" runat="server"></asp:TextBox><br /><br />

        <asp:Button ID="btnUpdate" runat="server" Text="שמור שינויים" OnClick="btnUpdate_Click" />
        <asp:Button ID="btnBack" runat="server" Text="חזרה ללוח" PostBackUrl="FoodBoard.aspx" CausesValidation="false" />
    </div>
</asp:Content>
