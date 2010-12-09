<%@ Page Title="Home" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Default" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
    <style type="text/css">
        #steamUserIdHeading
        {
            text-align: center;
            font-size: 200%;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="body">
    <form runat="server">
    <asp:HiddenField ID="steamUserIdHidden" runat="server" />
    </form>
    <h1>
        Home</h1>
    <div id="steamIdError" class="fberrorbox message" style="display: none;">
        You haven't set your Steam Community Profile URL. Please set it on the
        <steam:CanvasLink ID="steamIdErrorSettingsLink" runat="server" CanvasPage="Settings.aspx"
            Text="settings" />
        page.
        <steam:HelpLink ID="steamIdErrorHelpLink" runat="server" HelpAnchor="steam" />
    </div>
    <h1 id="steamUserIdHeading">
    </h1>
    <div id="publishPageMessage" class="fbinfobox message">
        Use the
        <steam:CanvasLink ID="publishPageMessageLink" runat="server" Text="publish" CanvasPage="Publish.aspx" />
        page to update and publish new achievements to your profile.
    </div>
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
    <script type="text/javascript" src="js/default.js"></script>
</asp:Content>
