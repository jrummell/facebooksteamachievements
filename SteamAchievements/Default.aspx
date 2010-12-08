<%@ Page Title="Home" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="body">
    <form runat="server">
    <asp:HiddenField ID="steamUserIdHidden" runat="server" />
    </form>
    <h1>
        Home</h1>
    <fieldset>
        <legend>Update</legend>
        <div id="noNewAchievementsMessage" class="fbinfobox message" style="display: none;">
            You have no new achievements.</div>
        <div id="achievementsUpdateFailure" class="fberrorbox message" style="display: none;">
            Your achievements could not be updated. Please verify that your Custom Url is correct
            and that your Steam Community Profile is public.
            <steam:HelpLink ID="updateFailedHelpLink" runat="server" HelpAnchor="steam" />
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
                Your games. This list includes all Steam games that you own (including some free
                Source Mods) that support achievements.
                <steam:HelpLink ID="gamesHelpLink" runat="server" HelpAnchor="games" />
            </p>
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
