<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AboutMe.aspx.cs" Inherits="AboutMe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>מפת האתר</h2>
    <ul>
        <li><a href="HomePage.aspx">דף הבית</a> – פתוח לכולם</li>
        <li><a href="AboutWebsite.aspx">אודות</a> – פתוח לכולם</li>
        <li><a href="regestaration.aspx">הרשמה</a> – לקוח חדש</li>
        <li><a href="login.aspx">כניסה</a> – מנהל / לקוח רשום</li>
        <li><a href="FoodBoard.aspx">לוח מודעות</a> – הצגה וחיפוש (SQL)</li>
        <li><a href="AddItem.aspx">הוספת מוצר</a> – ללקוח מחובר בלבד</li>
        <li><a href="EditItem.aspx">עדכון מוצר</a> – לבעל המוצר או מנהל</li>
        <li><a href="AdminPanel.aspx">פאנל מנהל</a> – מנהל אתר בלבד</li>
        <li><a href="Survey.aspx">סקר</a> – בונוס, ללקוח מחובר</li>
    </ul>
    <h3>הרשאות</h3>
    <ul>
        <li><strong>אורח</strong> – צפייה בדפים ציבוריים, הרשמה, כניסה</li>
        <li><strong>לקוח רשום</strong> – הוספה, עריכה ומחיקה של מוצרים שלו, סקר</li>
        <li><strong>מנהל אתר</strong> – admin / admin123 – גישה לפאנל ניהול</li>
    </ul>
</asp:Content>
