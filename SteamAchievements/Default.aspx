<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Default" %>
<%@ Import Namespace="SteamAchievements.Data"%>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="body">

    <h2>
        Steam Achievements</h2>
        <div>
    <p>
        <label for="steamIdTextBox">Steam Community ID</label><br />
        http://steamcommunity.com/id/
        <input id="steamIdTextBox" value="<%= SteamUserId %>"/>
        <span id="steamIdError" style="display:none; color:Red;">Required</span>
        <input type="button" value="Update" onclick="return updateSteamUserId();" />
    </p>
    </div>
    <div>
        <select id="gamesSelect" onchange="getAchievements();"></select>
    </div>
    <div id="achievementsDiv"></div>
    <asp:Repeater ID="achievementsRepeater" runat="server">
        <ItemTemplate>
            <div>
                <div style="float: left;">
                    <img src="<%# ((Achievement)Container.DataItem).ImageUrl %>" alt="" />
                </div>
                <div>
                    <strong>
                        <%# ((Achievement)Container.DataItem).Name %></strong>
                    <p>
                        <%# ((Achievement)Container.DataItem).Description %></p>
                </div>
            </div>
        </ItemTemplate>
        <SeparatorTemplate>
            <br style="clear: both;" />
        </SeparatorTemplate>
    </asp:Repeater>
    
    <script type="text/javascript">
        getGames();
    
        function getGames()
        {
            var ajax = new Ajax();
            ajax.responseType = Ajax.JSON;
            //ajax.requireLogin = true;
            ajax.ondone = function(data)
            {
                var gamesHtml = "<option>[select a game]</option>";

                for (var i = 0; i < data.length; i++)
                {
                    var a = data[i];
                    gamesHtml += "<option value='" + a.Id + "'>" + a.Name + "</option>";
                }

                var gamesSelect = document.getElementById("gamesSelect");
                gamesSelect.setInnerXHTML(gamesHtml);
            };
            ajax.onerror = function()
            {
                var gamesSelect = document.getElementById("gamesSelect");
                dialog = new Dialog(Dialog.DIALOG_CONTEXTUAL).setContext(gamesSelect).showMessage('Oops', 'A communication error occured. Try reloading the page.');
            };

            var parameters = {};
            ajax.post("http://www.rummell.info/SteamAchievements/Services/Achievement.svc/GetGames", parameters);
        }
    
        function getAchievements()
        {
            var ajax = new Ajax();
            ajax.responseType = Ajax.JSON;
            ajax.requireLogin = true;
            ajax.ondone = function(data)
            {
                var achievementsHtml = "";

                for (var i = 0; i < data.length; i++)
                {
                    var a = data[i];
                    achievementsHtml += "<div>";
                    achievementsHtml += "<div style='float: left;'><img src='" + a.ImageUrl + "' /></div>";
                    achievementsHtml += "<div><strong>" + a.Name + "</strong><p>" + a.Description + "</p></div>";
                    achievementsHtml += "</div>";
                }

                var achievementsDiv = document.getElementById("achievementsDiv");
                achievementsDiv.setInnerXHTML(achievementsHtml);
            };

            var steamUserId = document.getElementById("steamIdTextBox").getValue();
            var gameId = document.getElementById("gamesSelect").getValue();

            var parameters = { "steamUserId": steamUserId, "gameId": gameId };
            ajax.post("http://www.rummell.info/SteamAchievements/Services/Achievement.svc/GetAchievements", parameters);
        }
    
        function updateSteamUserId()
        {
            document.getElementById("steamIdError").setStyle("display", "none");
        
            var steamUserId = document.getElementById("steamIdTextBox").getValue();

            if (steamUserId == null || steamUserId == "")
            {
                document.getElementById("steamIdError").setStyle("display", "inline");
                return false;
            }

            //TODO: update achievements with web service
            var ajax = new Ajax();
            ajax.responseType = Ajax.JSON;
            ajax.requireLogin = true;
            ajax.ondone = function(data) { /* display update success message */ };

            var parameters = { "facebookUserId": null, "steamUserId": steamUserId };
            ajax.post("http://www.rummell.info/SteamAchievements/Services/Achievement.svc/UpdateSteamId", parameters);
        }
    </script>
</asp:Content>
