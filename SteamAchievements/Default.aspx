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
    
    <div id="debugDiv">[no error?]</div>
    
    <script src="http://www.rummell.info/SteamAchievements/default.js"></script>
    <script>
        getGames();
    </script>
    
    <fb:local-proxy/>
</asp:Content>
