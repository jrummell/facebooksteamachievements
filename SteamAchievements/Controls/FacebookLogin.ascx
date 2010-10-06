<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FacebookLogin.ascx.cs"
    Inherits="SteamAchievements.Controls.FacebookLogin" %>
<script src="http://connect.facebook.net/en_US/all.js" type="text/javascript"></script>
<script type="text/javascript">
    FB.init({
        appId: "<%= FacebookClientId %>",
        cookie: true,
        status: true,
        xfbml: true
    });

    FB.Event.subscribe('auth.login', function (response)
    {
        // Reload the application in the logged-in state
        window.top.location = 'http://apps.facebook.com/<%= FacebookUrlSuffix %>/';
    });
</script>
<% if (!IsLoggedIn)
   {%>
<div id="facebookLogin">
    <fb:login-button>
    </fb:login-button>
</div>
<%
    }%>
