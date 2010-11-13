/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />

$(document).ready(function ()
{
    getNewAchievements();

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

function getNewAchievements()
{
    $("#newAchievementsLoading").show();

    callAjax("UpdateAchievements", { steamUserId: _steamUserId }, function (updateCount)
    {
        callAjax("GetNewAchievements", { steamUserId: _steamUserId },
                        function (achievements)
                        {
                            displayAchievements(achievements);
                        });
    });
}

var _newAchievements = new Array();
function displayAchievements(achievements)
{
    _newAchievements = new Array();
    var achievementHtml = "\n";
    var gameId = new String();
    $(achievements).each(function (i)
    {
        var achievement = achievements[i];
        _newAchievements.push(achievement);

        if (gameId != achievement.Game.Id)
        {
            achievementHtml += "<h3>" + achievement.Game.Name + "\</h3>";
            gameId = achievement.Game.Id;
        }

        achievementHtml += "<div class='achievement'><input value='" + achievement.Id + "' type='checkbox' \/>";
        achievementHtml += "<img src='" + achievement.ImageUrl + "' alt='" + achievement.Description + "' \/>";
        achievementHtml += "<span class='description'>" + achievement.Description + "<\/span><\/div>\n";
    });

    $("#newAchievements").html(achievementHtml);

    // allow user to select only 5 achievements since only 5 images can be displayed at a time
    $("#newAchievements .achievement :checkbox, #newAchievements .achievement img").click(function ()
    {
        $(this).parents(".achievement").toggleClass("selected");
        if (this.tagName == "IMG")
        {
            var checkbox = $(this).prev()[0];
            checkbox.checked = !checkbox.checked;
        }

        var $checked = $("#newAchievements :checked");
        var disableUnchecked = $checked.length >= 5;
        $("#newAchievements .achievement :checkbox").filter(function (index)
        {
            return !this.checked;
        }).attr("disabled", disableUnchecked);
    });

    if (achievements.length == 0)
    {
        $("#noUnpublishedMessage").show();
    }
    else
    {
        $("#publishSelectedButton").show();
    }

    $("#newAchievementsLoading").hide();
}

function publishAchievements(achievements)
{
    // display publish dialog

    var images = new Array();
    var description = new String();
    var gameId = new String();

    $.each(achievements, function (i)
    {
        var achievement = achievements[i];
        images.push({
            type: 'image',
            src: achievement.ImageUrl,
            href: achievement.Game.StatsUrl
        });

        if (gameId != achievement.Game.Id)
        {
            gameId = achievement.Game.Id;

            if (i > 0 && description.length > 2)
            {
                // replace last comma with period
                description = description.substring(0, description.length - 2);
                description += ". ";
            }

            description += achievement.Game.Name + ": ";
        }

        description += achievement.Name;

        if (i < achievements.length - 1)
        {
            description += ", ";
        }
        else
        {
            description += ".";
        }
    });

    var publishParams = {
        method: 'stream.publish',
        attachment: {
            name: _steamUserId + " has earned new achievements",
            description: description,
            href: "http://steamcommunity.com/id/" + _steamUserId,
            media: images
        }
    };

    FB.ui(publishParams, function (response)
    {
        if (response && response.post_id)
        {
            // on successful publish, update published field on each published achievement.

            var achievementIds = new Array();
            for (i = 0; i < achievements.length; i++)
            {
                achievementIds.push(achievements[i].Id);
            }

            var data = { steamUserId: _steamUserId, achievementIds: achievementIds };
            callAjax("PublishAchievements", data, function () { getNewAchievements(); });
        }
    });
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