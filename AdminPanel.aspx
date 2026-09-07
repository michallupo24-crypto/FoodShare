<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdminPanel.aspx.cs" Inherits="AdminPanel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>פאנל מנהל אתר</h2>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>

    <h3>משתמשים רשומים</h3>
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
        DataKeyNames="id" OnRowCommand="gvUsers_RowCommand" EmptyDataText="אין משתמשים." CssClass="grid-table">
        <Columns>
            <asp:BoundField DataField="UserName" HeaderText="שם משתמש" />
            <asp:BoundField DataField="FirstName" HeaderText="שם פרטי" />
            <asp:BoundField DataField="LastName" HeaderText="שם משפחה" />
            <asp:BoundField DataField="City" HeaderText="עיר" />
            <asp:BoundField DataField="loginCount" HeaderText="כניסות" />
            <asp:CheckBoxField DataField="isAdmin" HeaderText="מנהל" ReadOnly="True" />
            <asp:TemplateField HeaderText="פעולות">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl='<%# "EditUser.aspx?id=" + Eval("id") %>' Text="עריכה" />
                    <asp:Button ID="btnDelUser" runat="server" Text="מחק" CommandName="DelUser"
                        CommandArgument='<%# Eval("id") %>'
                        OnClientClick="return confirm('למחוק משתמש?');"
                        Visible='<%# !(bool)Eval("isAdmin") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>