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
            <span id="steamIdError" class="error" style="display: none;">Required</span> <a href="#"
                onclick="return updateSteamUserId();">Update</a>
            <div id="updatingSteamUserId" class="loading" style="display: none;">
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend>Achievements</legend>
        <div>
            <a href="#" onclick="updateAchievements(); return false;">Update</a>
            <div id="updatingAchievements" class="loading" style="display: none;">
            </div>
        </div>
        <div>
            <select id="gamesSelect" onchange="getAchievements();">
                <option>Loading ...</option>
            </select>
        </div>
        <div id="loadingAchievements" class="loading" style="display: none;">
        </div>
        <div id="achievementsDiv">
        </div>
    </fieldset>
    <div id="log" class="fberrorbox" style="display: none;">
        <asp:Literal ID="errorLiteral" runat="server" />
    </div>

    <script type="text/javascript" src="default.js"></script>

    <script type="text/javascript">
        $(document).ready(function() { getGames(); });
    </script>

</asp:Content>
