<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SearchUser.aspx.cs" Inherits="SearchUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>חיפוש משתמשים</h2>
        <asp:Label ID="lblMessage" runat="server"></asp:Label>

        <label>חיפוש לפי שם משתמש:</label><br />
        <asp:TextBox ID="txtSearchName" runat="server" placeholder="הזיני שם משתמש..."></asp:TextBox>

        <label>חיפוש לפי עיר:</label><br />
        <asp:DropDownList ID="ddlCity" runat="server">
            <asp:ListItem Value="" Text="הכל" Selected="True" />
            <asp:ListItem Value="Tel Aviv" Text="תל אביב" />
            <asp:ListItem Value="Jerusalem" Text="ירושלים" />
            <asp:ListItem Value="Haifa" Text="חיפה" />
            <asp:ListItem Value="Beersheba" Text="באר שבע" />
        </asp:DropDownList><br /><br />

        <asp:Button ID="btnSearch" runat="server" Text="חפש" OnClick="btnSearch_Click" />
        <asp:Button ID="btnShowAll" runat="server" Text="הצג הכל" OnClick="btnShowAll_Click" CausesValidation="false" /><br /><br />
        <asp:Label ID="specificquestions" runat="server" Text="" Font-Bold="true"></asp:Label>
        <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False"
            EmptyDataText="לא נמצאו משתמשים." CssClass="grid-table">
            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="שם משתמש" />
                <asp:BoundField DataField="FirstName" HeaderText="שם פרטי" />
                <asp:BoundField DataField="LastName" HeaderText="שם משפחה" />
                <asp:BoundField DataField="City" HeaderText="עיר" />
                <asp:BoundField DataField="loginCount" HeaderText="כניסות" />
            </Columns>
        </asp:GridView>

    </div>
</asp:Content>
