<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="HomePage.aspx.cs" Inherits="HomePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="text-align:center; padding: 20px;">
        <h1 id="mainTitle">פרויקט שיתוף מוצרים</h1>
       
        <img id="logoImg" src="Images/school_logo.png" alt="סמל בית ספר" 
             style="width:150px; transition: 0.5s;" />
             
        <h2 >שם: מיכל רחל לופוביץ</h2>
        <p >ברוכים הבאים לאתר שנועד לצמצם בזבוז מזון ולעזור ביוקר המחייה.</p>
    </div>

    <script type="text/javascript">
        
        var logo = document.getElementById("logoImg");
        
        logo.onmouseover = function() {
            this.style.width = "200px"; 
            this.style.cursor = "pointer";
        };
        
        logo.onmouseout = function() {
            this.style.width = "150px"; 
        };

        
        var title = document.getElementById("mainTitle");
        title.onclick = function() {
            this.style.color = "blue";
            this.innerHTML = "תודה שלחצת על הכותרת!";
        };

       
    </script>
</asp:Content>