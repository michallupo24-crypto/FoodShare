<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>כניסה למערכת</h2>

        <label>אימייל:</label><br />
        <input type="email" name="Email" required /><br />

        <label>סיסמה:</label><br />
        <input type="password" name="Password" required /><br /><br />

        <input type="submit" name="mySubmit" value="כניסה" />
        <br /><br />
        <a href="regestaration.aspx">עדיין לא רשומים? לחצו כאן להרשמה</a>
    </div>
</asp:Content>
