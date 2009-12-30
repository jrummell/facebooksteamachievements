<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs"
    Inherits="SteamAchievements.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="body">
    <fieldset>
        <legend>Steam Community ID</legend>
        <input id="facebookUserIdHidden" type="hidden" value="<%= FacebookUserId %>" />
        <div>
            http://steamcommunity.com/id/
            <input id="steamIdTextBox" value="<%= SteamUserId %>" />
            <span id="steamIdError" style="display: none; color: Red;">Required</span> <a href="#"
                onclick="return updateSteamUserId();">Update</a> <span id="updatingSteamUserId" style="display: none;">
                    Updating ...</span>
        </div>
    </fieldset>
    <fieldset>
        <legend>Achievements</legend>
        <div>
            <a href="#" onclick="updateAchievements(); return false;">Update</a> <span id="updatingAchievements"
                style="display: none;">Updating ...</span>
        </div>
        <div>
            <select id="gamesSelect" onchange="getAchievements();">
                <option>Loading ...</option>
            </select>
        </div>
        <div id="achievementsDiv">
        </div>
    </fieldset>
    <div id="log" class="fberrorbox" style="display: none;">
    </div>

    <script type="text/javascript" src="default.js"></script>

    <script type="text/javascript">
        $(document).ready(function() { getGames(); });
    </script>

</asp:Content>
