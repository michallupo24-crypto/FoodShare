<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EditUser.aspx.cs" Inherits="EditUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>עריכת משתמש (מנהל)</h2>
        <asp:Label ID="lblMessage" runat="server"></asp:Label><br />

        <label>שם פרטי:</label><br />
        <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox><br />
        <label>שם משפחה:</label><br />
        <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox><br />
        <label>שם משתמש:</label><br />
        <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox><br />
        <label>אימייל:</label><br />
        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox><br />
        <label>סיסמה (השאירו ריק כדי לא לשנות):</label><br />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox><br />
        <label>קידומת:</label><br />
        <asp:TextBox ID="txtPhonePrefix" runat="server"></asp:TextBox><br />
        <label>טלפון:</label><br />
        <asp:TextBox ID="txtPhoneNumber" runat="server"></asp:TextBox><br />
        <label>שנת לידה:</label><br />
        <asp:TextBox ID="txtBirthYear" runat="server"></asp:TextBox><br />
        <label>מגדר:</label><br />
        <asp:TextBox ID="txtGender" runat="server"></asp:TextBox><br />
        <label>עיר מגורים:</label><br />
        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox><br />
        <label>מספר כניסות:</label><br />
        <asp:TextBox ID="txtLoginCount" runat="server"></asp:TextBox><br />
        <asp:CheckBox ID="chkIsAdmin" runat="server" Text="מנהל אתר" /><br /><br />

        <asp:Button ID="btnSave" runat="server" Text="שמור" OnClick="btnSave_Click" />
        <asp:Button ID="btnBack" runat="server" Text="חזרה" PostBackUrl="AdminPanel.aspx" CausesValidation="false" />
    </div>
</asp:Content>
