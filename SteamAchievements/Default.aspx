<%@ Page Title="Home" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Default.aspx.cs" Inherits="SteamAchievements.Default" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
    <style type="text/css">
        #steamUserIdHeading
        {
            font-size: 200%;
        }
        #profileImage
        {
            width: 69px;
            height: 69px;
            padding: 0 5px 5px 0;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="body">
    <form runat="server">
    <asp:HiddenField ID="steamUserIdHidden" runat="server" />
    </form>
    <div id="steamIdError" class="fberrorbox message" style="display: none;">
        You haven't set your Steam Community Profile URL. Please set it on the
        <steam:CanvasLink ID="steamIdErrorSettingsLink" runat="server" CanvasPage="Settings.aspx"
            Text="settings" />
        page.
        <steam:HelpLink ID="steamIdErrorHelpLink" runat="server" HelpAnchor="Configure_your_Steam_Community_Profile" />
    </div>
    <div id="heading" class="hidden">
        <asp:Image ID="profileImage" runat="server" ImageAlign="Left" />
        <h1>
            <asp:Label ID="steamUserIdHeading" runat="server" /></h1>
        <asp:Label ID="headlineLabel" runat="server" />
        <br class="clear" />
    </div>
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
                <steam:HelpLink ID="gamesHelpLink" runat="server" HelpAnchor="Supported_Games" />
            </p>
            <img id="loadingGames" class="loading" src="images/ajax-loader.gif" alt="Loading..." />
        </div>
        <div id="gamesDiv" class="games">
        </div>
        <script id="gamesTemplate" type="text/x-jquery-tmpl">
            <li class="game">
                <a target="_blank" href="${StoreUrl}"><img src="${ImageUrl}" alt="${Name}" title="${Name}" /></a><br />
                <a target="_blank" href="${StatsUrl}?tab=achievements">View Achievements</a>
            </li>
        </script>
    </fieldset>
    <div id="log" class="fberrorbox" style="display: none;">
        <asp:Literal ID="errorLiteral" runat="server" />
    </div>
    <script type="text/javascript" src="js/default.js"></script>
</asp:Content>
