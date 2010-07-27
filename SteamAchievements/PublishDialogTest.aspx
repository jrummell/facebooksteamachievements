<%@ Page Title="" Language="C#" AutoEventWireup="false" CodeBehind="PublishDialogTest.aspx.cs"
    Inherits="SteamAchievements.PublishDialogTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<head runat="server">
    <title></title>
</head>
<body>
    <div id="fb-root">
    </div>
    <% if (IsLoggedIn)
       {%>
    <input id="publishButton" type="button" value="Publish" onclick="publishTest();" />
    <%
        }
       else
       {%>
    <div id="facebookLogin">
        <fb:login-button>
        </fb:login-button>
    </div>
    <%
        }%>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script src="http://connect.facebook.net/en_US/all.js" type="text/javascript"></script>
    <script type="text/javascript">
        FB.init(
        {
            appId: "<%= FacebookClientId %>",
            cookie: true,
            status: true,
            xfbml: true
        });

        FB.Event.subscribe('auth.login', function (response)
        {
            // Reload the application in the logged-in state
            window.top.location = 'http://apps.facebook.com/<%= FacebookUrlSuffix %>/PublishDialogTest.aspx';
        });

        function publishTest()
        {
            var images = [
            {
                type: 'image',
                src: 'http://media.steampowered.com/steamcommunity/public/images/apps/240/7bbd09bac4ebe17b84e8fb0eb5d9e3351fcb4bc0.jpg',
                href: 'http://steamcommunity.com/id/nullreference/stats/CS:S?tab=achievements'
            },
            {
                type: 'image',
                src: 'http://media.steampowered.com/steamcommunity/public/images/apps/240/a646cdeba4eb3590fb1b16241949daec306df333.jpg',
                href: 'http://steamcommunity.com/id/nullreference/stats/CS:S?tab=achievements'
            }];

            var publishParams = {
                method: 'stream.publish',
                message: 'Steam Achievements Test',
                attachment: {
                    name: 'Test',
                    caption: 'A test caption',
                    description: 'Testing steam.publish from Steam Achievements Test',
                    href: 'http://apps.facebook.com/steamachievementsxt/',
                    media: images
                }
            };

            FB.ui(publishParams);
        }
    </script>
</body>
</html>
