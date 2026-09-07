<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Map.aspx.cs" Inherits="MapPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>מפת זמינות מזון</h2>
    <p class="chat-item-context">גודל הכתם וצבעו משקפים כמה מוצרים זמינים יש כרגע בכל עיר. לחיצה פותחת את הלוח מסונן לעיר הזו.</p>

    <div id="foodMap" class="food-map"></div>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.css" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.js"></script>
    <script type="text/javascript">
        (function () {
            var cities = <%= CitiesJson %>;

            var map = L.map('foodMap').setView([31.9, 34.9], 7);
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; OpenStreetMap contributors',
                maxZoom: 18
            }).addTo(map);

            cities.forEach(function (city) {
                var radius = 10 + city.count * 5;
                var color = city.count > 0 ? '#27ae60' : '#aaaaaa';

                var circle = L.circleMarker([city.lat, city.lon], {
                    radius: radius,
                    color: color,
                    fillColor: color,
                    fillOpacity: 0.45,
                    weight: 2
                }).addTo(map);

                var popupHtml = '<div style="text-align:right; direction:rtl;">' +
                    '<strong>' + city.he + '</strong><br/>' +
                    city.count + ' מוצרים זמינים';
                if (city.distance !== null) {
                    popupHtml += '<br/>' + city.distance + ' ק"מ ממך';
                }
                popupHtml += '<br/><a href="FoodBoard.aspx?city=' + encodeURIComponent(city.en) + '">לצפייה בלוח</a>' +
                    '</div>';

                circle.bindPopup(popupHtml);
            });
        })();
    </script>
</asp:Content>
