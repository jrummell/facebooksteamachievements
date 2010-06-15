<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="Help.aspx.cs" Inherits="SteamAchievements.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>
        How to use Steam Achievements</h1>
    <br />
    <h2>
        <a id="steam"></a>Configure your Steam Community profile</h2>
    <p>
        You need to first configure your Steam Community profile at <a href="https://steamcommunity.com/"
            target="_blank">https://steamcommunity.com/</a>. There is a <a href="https://support.steampowered.com/kb_article.php?ref=8882-BMXL-0801"
                target="_blank">Steam Community KB article</a> with detailed instructions
        and a video on how to do this. You must also set your Profile Status to Public.</p>
    <p>
        Make a note of your Custom URL suffix:<br />
        <img src="images/profilecustomurl.png" alt="http://steamcommunity.com/id/[custom url suffix]" />
    </p>
    <h2>
        <a id="app"></a>Configure Steam Achievements</h2>
    <p>
        On the Steam Achievements application page, complete your Custom URL and hit Update
        URL.<br />
        <img src="images/appcustomurl.png" alt="http://steamcommunity.com/id/[custom url suffix]" />
    </p>
    <p>
        Update your achievements by clicking Update Achievements in the Achievements box.
        This will find all of your Steam achievements for the supported games. It will also
        post that last five achievements that you earned to your profile:</p>
    <p>
        <img src="images/achievements.png" alt="" /></p>
    <p>
        Choose a game in the drop down list to view your achievements.</p>
    <p>
        Note: If you change your Steam Community Custom URL, you'll need to also update
        it in the Steam Achievements application and then update your achievements.</p>
    <p>
        <a href="Default.aspx?<%=Request.QueryString%>">Back to Steam Achievements</a></p>
</asp:Content>
