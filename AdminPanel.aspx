<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AdminPanel.aspx.cs" Inherits="AdminPanel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>פאנל מנהל אתר</h2>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>

    <h3>משתמשים רשומים</h3>
    <p class="chat-item-context">שורות מסומנות ⚠ = נקודות אמינות נמוכות או יותר משני דירוגי כוכב 1 - מומלץ לבדוק ולהחליט אם לחסום.</p>
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
        DataKeyNames="id" OnRowCommand="gvUsers_RowCommand" EmptyDataText="אין משתמשים." CssClass="grid-table"
        OnRowDataBound="gvUsers_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <%# (bool)Eval("Flagged") ? "⚠" : "" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UserName" HeaderText="שם משתמש" />
            <asp:BoundField DataField="FirstName" HeaderText="שם פרטי" />
            <asp:BoundField DataField="LastName" HeaderText="שם משפחה" />
            <asp:BoundField DataField="City" HeaderText="עיר" />
            <asp:BoundField DataField="loginCount" HeaderText="כניסות" />
            <asp:BoundField DataField="TrustPoints" HeaderText="נקודות אמינות" />
            <asp:BoundField DataField="OneStarCount" HeaderText="דירוגי 1 כוכב" />
            <asp:CheckBoxField DataField="isAdmin" HeaderText="מנהל" ReadOnly="True" />
            <asp:CheckBoxField DataField="IsBlocked" HeaderText="חסום" ReadOnly="True" />
            <asp:TemplateField HeaderText="פעולות">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl='<%# "EditUser.aspx?id=" + Eval("id") %>' Text="עריכה" />
                    <asp:Button ID="btnDelUser" runat="server" Text="מחק" CommandName="DelUser"
                        CommandArgument='<%# Eval("id") %>'
                        OnClientClick="return confirm('למחוק משתמש?');"
                        Visible='<%# !(bool)Eval("isAdmin") %>' />
                    <asp:Button ID="btnBlock" runat="server" Text="חסום" CommandName="BlockUser"
                        CommandArgument='<%# Eval("id") %>'
                        OnClientClick="return confirm('לחסום את המשתמש/ת?');"
                        Visible='<%# !(bool)Eval("isAdmin") && !(bool)Eval("IsBlocked") %>' />
                    <asp:Button ID="btnUnblock" runat="server" Text="בטל חסימה" CommandName="UnblockUser"
                        CommandArgument='<%# Eval("id") %>'
                        Visible='<%# (bool)Eval("IsBlocked") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <asp:Panel ID="pnlReports" runat="server">
        <h3>דיווחים על תמונות (בהמתנה)</h3>
        <asp:GridView ID="gvReports" runat="server" AutoGenerateColumns="False"
            DataKeyNames="ReportId" OnRowCommand="gvReports_RowCommand" EmptyDataText="אין דיווחים בהמתנה." CssClass="grid-table">
            <Columns>
                <asp:BoundField DataField="ItemName" HeaderText="מוצר" />
                <asp:BoundField DataField="Reason" HeaderText="סיבה" />
                <asp:BoundField DataField="CreatedAt" HeaderText="תאריך" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:TemplateField HeaderText="פעולות">
                    <ItemTemplate>
                        <asp:Button ID="btnUphold" runat="server" Text="אשר תלונה (התמונה נשארת מוסתרת)" CommandName="Uphold" CommandArgument='<%# Eval("ReportId") %>' />
                        <asp:Button ID="btnDismiss" runat="server" Text="בטל תלונה (שחזר תמונה)" CommandName="Dismiss" CommandArgument='<%# Eval("ReportId") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
