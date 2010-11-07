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

</script>
<% if (!IsLoggedIn)
   {%>
<div id="facebookLogin">
    <a id="loginButton" class="button" href="#">Login</a>
</div>
<script type="text/javascript">
    $(document).ready(function ()
    {
        $("#loginButton").click(function ()
        {
            FB.login(function (response)
            {
                if (response.session)
                {
                    if (response.perms)
                    {
                        // user is logged in and granted some permissions.
                        // perms is a comma separated list of granted permissions
                        // Reload the application in the logged-in state
                        window.top.location = 'http://apps.facebook.com/<%= FacebookUrlSuffix %>/';
                    }
                    else
                    {
                        // user is logged in, but did not grant any permissions
                    }
                }
                else
                {
                    // user is not logged in
                }
            }, { perms: 'publish_stream,offline_access' });
        });
    });
</script>
<%
    }%>
