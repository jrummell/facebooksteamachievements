<%@ Page Title="" Language="C#" AutoEventWireup="false" CodeBehind="PublishDialogTest.aspx.cs"
    Inherits="SteamAchievements.PublishDialogTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://www.facebook.com/2008/fbml">
<head runat="server">
    <title></title>
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
</head>
<body>
    <!-- 
        <asp:PlaceHolder ID="cookieValuesPlaceHolder" runat="server" />
    -->
    <div id="fb-root">
    </div>
    <% if (IsLoggedIn)
       {%>
    <form action="PublishDialogTest.aspx">
    <input id="getNewAchievementsButton" type="button" value="Get New Achievements" />
    <div id="newAchievements">
    </div>
    <input id="publishSelectedButton" type="button" value="Publish Selected" style="display: none;" />
    </form>
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

        $(document).ready(function ()
        {
            $("#getNewAchievementsButton").click(getNewAchievements);

            $("#publishSelectedButton").click(function ()
            {
                var achievementsToPublish = new Array();
                $("#newAchievements input:checked").each(function ()
                {
                    var achievementId = this.value;
                    var matched = $.grep(_newAchievements, function (achievement, i)
                    {
                        return achievement.Id == achievementId;
                    });

                    $.each(matched, function (i, achievement)
                    {
                        achievementsToPublish.push(achievement);
                    });
                });

                if (achievementsToPublish.length > 0)
                {
                    publishAchievements(achievementsToPublish);
                }
            });
        });

        var _steamUserId = "<%= SteamUserId %>";
        function getNewAchievements(callback)
        {
            callAjax("UpdateAchievements", { steamUserId: _steamUserId }, function (updateCount)
            {
                callAjax("GetNewAchievements", { steamUserId: _steamUserId },
                        function (achievements) { displayAchievements(achievements); });
            });
        }

        var _newAchievements = new Array();
        function displayAchievements(achievements)
        {
            _newAchievements = new Array();
            var achievementHtml = "\n";

            $(achievements).each(function (i)
            {
                var achievement = achievements[i];
                _newAchievements.push(achievement);

                achievementHtml += "<div class='achievement'><input value='" + achievement.Id + "' type='checkbox' \/>";
                achievementHtml += "<img src='" + achievement.ImageUrl + "' alt='" + achievement.Description + "' \/>";
                achievementHtml += "<span class='description'>" + achievement.Description + "<\/span><\/div>\n";
            });

            $("#newAchievements").html(achievementHtml);

            //TODO: allow user to select only 5 achievements since only 5 images can be displayed at a time?
            $("#newAchievements .achievement input, #newAchievements .achievement img").click(function ()
            {
                $(this).parents(".achievement").toggleClass("selected");
                if (this.tagName == "IMG")
                {
                    var checkbox = $(this).prev()[0];
                    checkbox.checked = !checkbox.checked;
                }

                var $checked = $("#newAchievements :checked");
                var disableUnchecked = $checked.length >= 5;
                $("#newAchievements :unchecked").each(function ()
                {
                    this.disabled = disableUnchecked;
                });
            });

            $("#publishSelectedButton").show();
        }

        function publishAchievements(achievements)
        {
            //TODO: display publish dialog. on successful publish, update published field on each published achievement.

            var images = new Array();

            $.each(achievements, function (i)
            {
                var achievement = achievements[i];
                images.push({
                    type: 'image',
                    src: achievement.ImageUrl,
                    href: achievement.Game.StatsUrl
                });
            });

            var publishParams = {
                method: 'stream.publish',
                message: 'Steam Achievements',
                attachment: {
                    name: 'Test',
                    caption: 'A test caption',
                    description: 'Testing steam.publish from Steam Achievements Test',
                    href: 'http://apps.facebook.com/steamachievementsx/',
                    media: images
                }
            };

            FB.ui(publishParams);
        }

        var _serviceBase = "Services/Achievement.svc/";
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
