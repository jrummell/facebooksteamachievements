<%@ Page Title="Help" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="Help.aspx.cs" Inherits="SteamAchievements.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #toc
        {
            padding-top: 1em;
            padding-bottom: 1em;
        }
        #toc a
        {
            padding-left: 1em;
            display: block;
        }
        #helpContents h2 a
        {
            color: #000;
            text-decoration: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>
        Help</h1>
    <br />
    <div>
        <h2>
            Contents</h2>
        <p id="toc">
        </p>
    </div>
    <div id="helpContents">
        <h2>
            <a id="steam">Configure your Steam Community Profile</a></h2>
        <p>
            You need to first configure your Steam Community profile at <a href="https://steamcommunity.com/"
                target="_blank">https://steamcommunity.com/</a>. There is a <a href="https://support.steampowered.com/kb_article.php?ref=8882-BMXL-0801"
                    target="_blank">Steam Community KB article</a> with more information. You
            must also set your Profile Status to Public.</p>
        <p>
            Make a note of your Custom URL suffix:<br />
            <img src="images/profilecustomurl.png" alt="http://steamcommunity.com/id/[custom url suffix]" />
        </p>
        <p>
            Note: If you change your Steam Community Custom URL, you'll need to also update
            it in the Steam Achievements application.</p>
        <h2>
            <a id="app">Configure Steam Achievements</a></h2>
        <p>
            On the Settings page, complete your Custom URL and hit <b>Save</b>.<br />
            <img src="images/appcustomurl.png" alt="http://steamcommunity.com/id/[custom url suffix]" />
        </p>
        <p>
            To have your achievements published automatically, check the <b>Auto Update</b>
            box and hit <b>Save</b>. This will publish any new achievements you have earned
            once a day.</p>
        <h2>
            <a id="update">Publish Your Achievements</a></h2>
        <p>
            Publish your achievements on the <b>Publish</b> page. Select up to five achievements
            and click <b>Publish Selected</b>. The post on your wall will look like this:</p>
        <p>
            <img src="images/achievements.png" alt="" /></p>
        <h2>
            <a id="view">View Your Achievements</a></h2>
        <p>
            Click the <b>View Achievements</b> link under any game in the Games box to view
            your achievements.</p>
        <h2>
            <a id="games">Supported Games</a></h2>
        <p>
            All Steam games that have Steam achievements are supported. For a full list, please
            visit <a target="_blank" href="http://steamcommunity.com/stats/">http://steamcommunity.com/stats/</a>.
            Please note that this list <strong>does not</strong> include <em>Games for Windows Live</em>
            titles purchased from Steam as these games use the Xbox Live network.</p>
    </div>
    <script type="text/javascript">
        $(document).ready(function ()
        {
            $("h2 a", "#helpContents").each(function ()
            {
                var $this = $(this);
                $("#toc").append("<a href='#" + $this.attr("id") + "'>" + $this.parent().text() + "<\/a>");
            });

            $achievements.updateSize();

            var topic = '<%= Request["topic"] ?? String.Empty %>';
            if (topic != "")
            {
                $("#" + topic).focus();
            }
        });
    </script>
</asp:Content>
