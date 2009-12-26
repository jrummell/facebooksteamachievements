<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs"
    Inherits="SteamAchievements.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="body">
    <h1>
        Steam Achievements</h1>
    <div>
        <input id="facebookUserIdHidden" type="hidden" value="<%= FacebookUserId %>" />
        <p>
            <label for="steamIdTextBox">
                Steam Community ID</label><br />
            http://steamcommunity.com/id/
            <input id="steamIdTextBox" value="<%= SteamUserId %>" />
            <span id="steamIdError" style="display: none; color: Red;">Required</span>
            <input type="button" value="Update" onclick="return updateSteamUserId();" />
        </p>
        <p>
            (<a href="#" onclick="updateAchievements(); return false;">Update achievements</a>)
            <span id="updating" style="display: none;">Updating ...</span>
        </p>
    </div>
    <div>
        <select id="gamesSelect" onchange="getAchievements();">
        </select>
    </div>
    <div id="achievementsDiv">
    </div>
    <div id="log" class="fberrorbox" style="display: none;">
    </div>

    <script type="text/javascript" src="default.js"></script>

    <script type="text/javascript">
        $(document).ready(function() { getGames(); });
    </script>

</asp:Content>
