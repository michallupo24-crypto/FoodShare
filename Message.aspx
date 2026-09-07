<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Message.aspx.cs" Inherits="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>הודעה</h2>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        <br /><br />
        <a href="FoodBoard.aspx">חזרה ללוח המזון</a>
    </div>
</asp:Content>
