<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="ProfilePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="profile-header">
        <h2><%= Server.HtmlEncode(Username) %></h2>
        <% if (ReviewCount > 0) { %>
            <p class="profile-rating">&#9733; <%= AverageRating %> (<%= ReviewCount %> ביקורות)</p>
        <% } else { %>
            <p>עדיין אין ביקורות על המשתמש/ת הזה/זו.</p>
        <% } %>
    </div>

    <asp:Literal ID="litReviews" runat="server"></asp:Literal>
</asp:Content>
