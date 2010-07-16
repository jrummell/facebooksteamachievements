<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>
        How to use Steam Achievements</h1>
    <br />
    <div>
        <h2>
            Contents</h2>
        <p id="toc">
        </p>
    </div>
    <div id="helpContents">
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
            On the Steam Achievements application page, complete your Custom URL and hit <b>Update
                URL</b>.<br />
            <img src="images/appcustomurl.png" alt="http://steamcommunity.com/id/[custom url suffix]" />
        </p>
        <h2>
            <a id="update"></a>Update Your Achievements</h2>
        <p>
            Update your achievements by clicking <b>Update Achievements</b> in the Update box.
            This will update the achievements that Steam Achievements knows about. It will also
            post up to five of your updated achievements to your profile:</p>
        <p>
            <img src="images/achievements.png" alt="" /></p>
        <h2>
            <a id="view"></a>View Your Achievements</h2>
        <p>
            Click the <b>View Achievements</b> link under any game in the Games box to view
            your achievements.</p>
        <p>
            Note: If you change your Steam Community Custom URL, you'll need to also update
            it in the Steam Achievements application and then update your achievements.</p>
        <p>
            <a href="Default.aspx?<%=Request.QueryString%>">Back to Steam Achievements</a></p>
        <h2>
            <a id="A1"></a>Supported Games</h2>
        <p>
            All Steam games that have Steam achievements are supported. For a full list, please
            visit <a target="_blank" href="http://steamcommunity.com/stats/">http://steamcommunity.com/stats/</a>. 
            Please note that this list <strong>does not</strong> include <em>Games for
            Windows Live</em> titles purchased from Steam as these games use the Xbox Live 
            network.</p>
    </div>
    <script type="text/javascript">
        $(document).ready(function ()
        {
            $("h2 a", "#helpContents").each(function ()
            {
                var $this = $(this);
                $("#toc").append("<a href='#" + $this.attr("id") + "'>" + $this.parent().text() + "<\/a>");
            });
        });
    </script>
</asp:Content>
