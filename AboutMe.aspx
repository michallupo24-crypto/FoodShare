<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AboutMe.aspx.cs" Inherits="AboutMe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>מפת האתר</h2>
    <ul>
        <li><a href="HomePage.aspx">דף הבית</a> – פתוח לכולם</li>
        <li><a href="AboutWebsite.aspx">אודות</a> – פתוח לכולם</li>
        <li><a href="regestaration.aspx">הרשמה</a> – משתמש/ת חדש/ה</li>
        <li><a href="login.aspx">כניסה</a> – מנהל/ת / משתמש/ת רשום/ה</li>
        <li><a href="FoodBoard.aspx">לוח מודעות</a> – הצגה וחיפוש מוצרים</li>
        <li><a href="AddItem.aspx">הוספת מוצר</a> – למשתמש/ת מחובר/ת בלבד</li>
        <li><a href="EditItem.aspx">עריכת מוצר</a> – לבעל/ת המוצר או מנהל/ת</li>
        <li><a href="AdminPanel.aspx">פאנל מנהל</a> – מנהל/ת אתר בלבד</li>
        <li><a href="Survey.aspx">סקר</a> – למשתמש/ת מחובר/ת</li>
    </ul>
    <h3>הרשאות</h3>
    <ul>
        <li><strong>אורח/ת</strong> – צפייה בדפים ציבוריים, הרשמה, כניסה</li>
        <li><strong>משתמש/ת רשום/ה</strong> – הוספה, עריכה ומחיקה של מוצרים שלו/ה, סקר</li>
        <li><strong>מנהל/ת אתר</strong> – גישה לפאנל ניהול</li>
    </ul>
</asp:Content>
