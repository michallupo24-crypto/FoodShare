<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="FoodBoard.aspx.cs" Inherits="FoodBoard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>לוח שיתוף המזון</h2>
    <p>סינון לפי עיר איסוף, קטגוריה וזמן עד פג תוקף (אפשר לבחור אחד או יותר):</p>

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
            <asp:BoundField DataField="ItemName" HeaderText="שם המוצר" />
            <asp:BoundField DataField="Category" HeaderText="קטגוריה" />
            <asp:BoundField DataField="PickupCity" HeaderText="עיר איסוף" />
            <asp:BoundField DataField="PickupLocation" HeaderText="כתובת/מיקום" />
            <asp:BoundField DataField="ExpiryDate" HeaderText="תאריך תפוגה" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="DaysLeft" HeaderText="ימים לפג תוקף" />
            <asp:BoundField DataField="Quantity" HeaderText="כמות" />
            <asp:BoundField DataField="UserName" HeaderText="מפרסם" />
            <asp:TemplateField HeaderText="פעולות">
                <ItemTemplate>
                    <asp:HyperLink ID="lnkEdit" runat="server" NavigateUrl='<%# "EditItem.aspx?id=" + Eval("ItemID") %>'
                        Text="עריכה" Visible='<%# CanEdit(Eval("UserID")) %>' />
                    <asp:Button ID="btnDelete" runat="server" Text="מחיקה" CommandName="DeleteItem"
                        CommandArgument='<%# Eval("ItemID") %>'
                        Visible='<%# CanEdit(Eval("UserID")) %>'
                        OnClientClick="return confirm('למחוק את המוצר?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
