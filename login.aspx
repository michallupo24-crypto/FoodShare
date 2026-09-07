<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>כניסה למערכת</h2>
        <p>מנהל: admin / admin123 | לקוח: אחרי הרשמה</p>

        <label>שם משתמש:</label><br />
        <input type="text" name="UserName" required /><br />

        <label>סיסמה:</label><br />
        <input type="password" name="Password" required /><br /><br />

        <input type="submit" name="mySubmit" value="כניסה" />
        <br /><br />
        <a href="regestaration.aspx">עדיין לא רשומה? לחצי כאן להרשמה</a>
    </div>
</asp:Content>
