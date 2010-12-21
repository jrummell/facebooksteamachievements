<%@ Page Title="Publish" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Publish.aspx.cs" Inherits="SteamAchievements.Publish" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
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
        <steam:HelpLink ID="steamIdErrorHelpLink" runat="server" HelpAnchor="steam" />
    </div>
    <div id="achievementsUpdateFailure" class="fberrorbox message" style="display: none;">
        Your achievements could not be updated. Please verify that your Custom Url is correct
        and that your Steam Community Profile is public.
        <steam:HelpLink ID="updateFailedHelpLink" runat="server" HelpAnchor="steam" />
    </div>
    <fieldset>
        <legend>Unpublished Achievements</legend>
        <p>
            You can publish up to five achievements at a time.</p>
        <p>
            <a id="publishSelectedButton" class="button" href="#" style="display: none;">Publish
                Selected</a>
        </p>
        <div id="noUnpublishedMessage" class="fbinfobox message" style="display: none;">
            You have no unpublished achievements.</div>
        <img id="newAchievementsLoading" class="loading" src="images/ajax-loader.gif" alt="Getting unpublished achievements..." />
        <div id="newAchievements" class="unpublished">
            <script type="text/javascript">
                var _currentGameName = "";                
            </script>
            <script id="achievementTemplate" type="text/x-jquery-tmpl">
                {{if Game.Name != _currentGameName }}
                <h3 class="game">${_currentGameName = Game.Name}</h3>
                {{/if}}
                <div class="achievement">
                    <div class="left">
                        <input type="checkbox" value="${Id}" />
                        <img src="${ImageUrl}" alt="${Name}" />
                    </div>
                    <div class="text left">
                        <div class="name">${Name}</div>
                        <div class="description">${Description}</div>
                    </div>
                </div>
                <br class="clear" />
            </script>
        </div>
    </fieldset>
    </form>
    <div id="log">
    </div>
    <script type="text/javascript" src="js/publish.js"></script>
</asp:Content>
