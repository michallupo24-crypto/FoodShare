<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="regestaration.aspx.cs" Inherits="regestaration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="form-container">
        <h2>הצטרפות לקהילה</h2>

        <label>שם פרטי:</label><br />
        <input type="text" id="FirstName" name="FirstName" /><br />

        <label>שם משפחה:</label><br />
        <input type="text" name="LastName" /><br />

        <label>שם משתמש:</label><br />
        <input type="text" name="UserName" /><br />

        <label>אימייל:</label><br />
        <input type="email" name="Email" /><br />

        <label>סיסמה:</label><br />
        <input type="password" name="Password" /><br />

        <label>שנת לידה:</label><br />
        <input type="number" id="BirthYear" name="BirthYear" /><br />

        <label>מגדר:</label><br />
        <input type="radio" name="Gender" value="male" checked /> זכר
        <input type="radio" name="Gender" value="female" /> נקבה<br /><br />

        <label>קידומת טלפון:</label>
        <select name="PhonePrefix">
            <option value="050">050</option>
            <option value="052">052</option>
            <option value="053">053</option>
            <option value="054">054</option>
            <option value="055">055</option>
            <option value="058">058</option>
        </select>
        <label>מספר:</label>
        <input type="text" name="PhoneNumber" maxlength="7" /><br /><br />

        <label>עיר מגורים:</label><br />
        <select name="City">
            <option value="Tel Aviv">תל אביב</option>
            <option value="Jerusalem">ירושלים</option>
            <option value="Haifa">חיפה</option>
            <option value="Beersheba">באר שבע</option>
        </select><br /><br />

        <input type="hidden" id="Lat" name="Lat" />
        <input type="hidden" id="Lon" name="Lon" />
        <button type="button" onclick="shareLocation()">שיתוף מיקום לחישוב מרחק מדויק (לא חובה)</button>
        <span id="locationStatus" class="chat-item-context"></span>
        <br /><br />

        <input type="submit" name="mySubmit" value="הירשם" onclick="return checkForm()" />
    </div>

    <script type="text/javascript">
        function shareLocation() {
            var status = document.getElementById("locationStatus");
            if (!navigator.geolocation) {
                status.textContent = "הדפדפן לא תומך בשיתוף מיקום.";
                return;
            }
            status.textContent = "מבקש הרשאה...";
            navigator.geolocation.getCurrentPosition(function (pos) {
                document.getElementById("Lat").value = pos.coords.latitude;
                document.getElementById("Lon").value = pos.coords.longitude;
                status.textContent = "המיקום שותף בהצלחה. תוכלו לראות מרחק מדויק למוצרים.";
            }, function () {
                status.textContent = "שיתוף המיקום נכשל או נדחה - אפשר להירשם גם בלעדיו.";
            });
        }

        function checkForm() {
            var name = document.getElementById("FirstName").value;
            var year = document.getElementById("BirthYear").value;

            if (!/^[A-Za-z\u0590-\u05FF]+$/.test(name)) {
                alert("שם פרטי חייב להכיל אותיות בלבד!");
                return false;
            }
            if (year < 1900 || year > 2026) {
                alert("שנת לידה לא הגיונית!");
                return false;
            }
            return true;
        }
    </script>
</asp:Content>