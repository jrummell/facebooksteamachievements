<%@ Page Title="Publish" Language="C#" AutoEventWireup="false" MasterPageFile="~/Site.Master"
    CodeBehind="Publish.aspx.cs" Inherits="SteamAchievements.Publish" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head">
    <style type="text/css">
        #newAchievements .achievement
        {
            height: 70px;
            padding: 0px;
            margin: 5px;
            border: 1px solid #fff;
        }
        #newAchievements .selected
        {
            background-color: #fff9d7;
            border: 1px solid #e2c822;
        }
        #newAchievements .achievement input
        {
            margin-top: 25px;
            margin-bottom: 25px;
        }
        #newAchievements .achievement img
        {
            vertical-align: middle;
        }
        #newAchievements .achievement .description
        {
            vertical-align: middle;
            margin-left: 0.5em;
        }
    </style>
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
        <div id="newAchievements">
        </div>
    </fieldset>
    </form>
    <div id="log">
    </div>
    <script type="text/javascript" src="js/publish.js"></script>
</asp:Content>
