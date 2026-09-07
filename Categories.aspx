<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="text-align:center;">
        <h2>בחרו קטגוריית מזון</h2>
        
       
        <img id="gallery" src="Images/bread.jpg" style="width:300px; height:200px; cursor:pointer;" />
        <p>לחצו על התמונה כדי לראות את כל המוצרים בקטגוריה הנוכחית</p>
    </div>

    <script type="text/javascript">
        var images = ["bread.jpg", "vegetables.jpg", "dairy.jpg"];
        var categories = ["פחמימות", "ירקות ופירות", "מוצרי חלב"]; 
        var i = 0;

        var imgElement = document.getElementById("gallery");

    
        setInterval(function() {
            i = (i + 1) % images.length;
            imgElement.src = "Images/" + images[i];
        }, 2500);
        imgElement.onclick = function() {
            window.location.href = "FoodBoard.aspx?cat=" + categories[i];
        };
    </script>
</asp:Content>