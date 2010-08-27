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
    <input id="publishButton" type="button" value="Publish Test" onclick="publishTest();" />

    <input id="getNewAchievementsButton" type="button" value="Get New Achievements" />
    <div id="New Achievements"></div>
    <input id="publishSelectedButton" type="button" value="Publish Selected" style="display:none;" />

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
    <div id="log">
    </div>
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

        function getNewAchievements(callback)
        {
            callAjax("GetNewAchievements", { steamUserId: "<%= SteamUserId %>" }, function (result) { publishAchievements(result); });
        }

        function displayAchievements(achievements)
        {
            //TODO: display list of new achievements that the user can select (up to 5?) and then publish with the dialog
        }

        function publishAchievements(achievements)
        {
            //TODO: display publish dialog. on successful publish, update published field on each published achievement.
        }

        function callAjax(method, query, ondone, onerror)
        {
            if (onerror == null)
            {
                onerror = function (m)
                {
                    $("#log").text(m.Message).show();
                };
            }

            $.ajax({
                url: _serviceBase + method,
                data: JSON.stringify(query),
                type: "POST",
                processData: true,
                contentType: "application/json",
                timeout: 120000, // 2 minutes
                dataType: "json",
                success: ondone,
                error: function (xhr)
                {
                    if (!onerror)
                    {
                        return;
                    }

                    if (xhr.responseText)
                    {
                        try
                        {
                            var err = JSON.parse(xhr.responseText);
                            if (err)
                            {
                                onerror(err);
                            }
                            else
                            {
                                onerror({ Message: "Unknown server error." });
                            }
                        }
                        catch (e)
                        {
                            onerror({ Message: "Unknown server error." });
                        }
                    }
                    return;
                }
            });
        }
    </script>
</body>
</html>
