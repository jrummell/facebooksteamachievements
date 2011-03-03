<%@ Page Title="Publish" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Publish.aspx.cs" Inherits="SteamAchievements.Publish" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="body">
    <h1>
        Publish</h1>
    <form runat="server">
    <asp:HiddenField ID="steamUserIdHidden" runat="server" />
    <div id="steamIdError" class="fberrorbox message" style="display: none;">
        You haven't set your Steam Community Profile URL. Please set it on the
        <steam:CanvasLink ID="steamIdErrorSettingsLink" runat="server" CanvasPage="Settings.aspx"
            Text="settings" />
        page.
        <steam:HelpLink ID="steamIdErrorHelpLink" runat="server" HelpAnchor="Configure_your_Steam_Community_Profile" />
    </div>
    <div id="achievementsUpdateFailure" class="fberrorbox message" style="display: none;">
        Your achievements could not be updated. Please verify that your Custom Url is correct
        and that your Steam Community Profile is public.
        <steam:HelpLink ID="updateFailedHelpLink" runat="server" HelpAnchor="Configure_your_Steam_Community_Profile" />
    </div>
    <fieldset>
        <legend>Unpublished Achievements</legend>
        <p>
            Click an achievement icon to select it. You can publish up to five at a time.</p>
        <p>
            <a id="publishSelectedButton" class="button" href="#" style="display: none;">Publish
                Selected</a> (If you don't see the publish dialog after clicking, scroll down.)
        </p>
        <div id="noUnpublishedMessage" class="fbinfobox message" style="display: none;">
            You have no unpublished achievements.</div>
        <img id="newAchievementsLoading" class="loading" src="images/ajax-loader.gif" alt="Getting unpublished achievements..." />
        <ul id="newAchievements" class="unpublished">
        </ul>
        <script type="text/javascript">
            var _currentGameName = "";                
        </script>
        <script id="achievementTemplate" type="text/x-jquery-tmpl">
            <li class="achievement">
                <input type="checkbox" value="${Id}" class="hidden" />
                <img class="left" src="${ImageUrl}" alt="${Name}" />
                <div class="text">
                    <h3>${Game.Name}</h3>
                    <div class="name">${Name}</div>
                    <div class="description">${Description}</div>
                </div>
            </li>
        </script>
    </fieldset>
    </form>
    <div id="log">
    </div>
    <script type="text/javascript" src="js/publish.js"></script>
</asp:Content>
