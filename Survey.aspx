<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Survey.aspx.cs" Inherits="Survey" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>סקר – בונוס (XML)</h2>
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        
        <p>האם האתר עוזר לך לצמצם בזבוז מזון?</p>
        
        <asp:RadioButtonList ID="rblAnswer" runat="server">
            <asp:ListItem Text="כן מאוד" Value="כן מאוד" />
            <asp:ListItem Text="במידה מסוימת" Value="במידה מסוימת" />
            <asp:ListItem Text="לא ממש" Value="לא ממש" />
        </asp:RadioButtonList>
        
        <br />
        <asp:Button ID="btnSend" runat="server" Text="שלח תשובה" OnClick="btnSend_Click" />
        
        <hr />
        <h3>תוצאות הסקר:</h3>
        <asp:Literal ID="litResults" runat="server" />
    </div>
</asp:Content>