<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FacebookLogin.ascx.cs"
    Inherits="SteamAchievements.Controls.FacebookLogin" %>
<script src="http://connect.facebook.net/en_US/all.js" type="text/javascript"></script>
<script type="text/javascript">
    FB.init({
        appId: "<%= FacebookClientId %>",
        cookie: true,
        status: true,
        xfbml: false
    });
</script>
