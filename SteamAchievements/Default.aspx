<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs"
    Inherits="SteamAchievements.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="body">
    <fieldset>
        <legend>Steam Community Custom URL</legend>
        <input id="facebookUserIdHidden" type="hidden" value="<%=FacebookUserId%>" />
        <div id="steamIdUpdateSuccess" class="fbinfobox message" style="display: none;">
            Your URL has been updated.</div>
        <div>
            <p>
                http://steamcommunity.com/id/
                <input id="steamIdTextBox" value="<%=SteamUserId%>" />
                <span id="steamIdError" class="error" style="display: none;">Required</span> <a id="updateSteamIdButton"
                    class="button" href="#">
                    <img src="images/disk.png" alt="" />
                    Update URL</a> <a class="button" href="Help.aspx?<%=Request.QueryString%>">
                        <img src="images/help.png" alt="Help" align="middle" />
                        Help</a>
                <img id="updatingSteamUserId" class="loading" src="images/ajax-loader.gif" alt="Updating..." />
            </p>
        </div>
    </fieldset>
    <fieldset>
        <legend>Update</legend>
        <div id="achievementsUpdateSuccess" class="fbinfobox message" style="display: none;">
            Your achievements have been updated (<span id="newAchievementCount"></span> new).</div>
        <div id="achievementsUpdateFailure" class="fberrorbox message" style="display: none;">
            Your achievements could not be updated. Please verify that your Custom Url is correct
            and that your Steam Community Profile is public. <a href="Help.aspx?<%=Request.QueryString%>">
                Help</a>
        </div>
        <div>
            <p>
                Update your achievements and publish new ones on your wall. This won't publish more
                than five at a time.
            </p>
            <p>
                <a id="updateAchievementsButton" class="button" href="#">Update Achievements</a>
                <img id="updatingAchievements" class="loading" src="images/ajax-loader.gif" alt="Updating..." />
            </p>
        </div>
    </fieldset>
    <fieldset>
        <legend>Games</legend>
        <div>
            <p>
                Your Steam games that have achievements.</p>
            <img id="loadingGames" class="loading" src="images/ajax-loader.gif" alt="Loading..." />
        </div>
        <div id="gamesDiv" class="games">
        </div>
    </fieldset>
    <div id="log" class="fberrorbox" style="display: none;">
        <asp:Literal ID="errorLiteral" runat="server" />
    </div>

    <script type="text/javascript" src="default.js"></script>

</asp:Content>
