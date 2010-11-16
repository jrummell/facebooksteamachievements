﻿/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />
var _serviceBase = "Services/Achievement.svc/";
var _log = false;

$(document).ready(function ()
{
    init();
    getGames();
});

function init()
{
    $("img.loading").hide();

    // hide messages on click
    $("div.message").append(" <span class='dismiss'>Click to dismiss.</span>").click(function ()
    {
        $(this).hide('normal');
    });

    $("#updateSteamIdButton").click(function ()
    {
        var result = updateSteamUserId();

        if (result == false)
        {
            return false;
        }

        getGames();
    });

    $("#updateAchievementsButton").click(function ()
    {
        updateAchievements();
        return false;
    });
}

function getGames()
{
    if (!validateSteamUserId())
    {
        return false;
    }

    var updatingSelector = "#updatingGames";
    showLoading(updatingSelector);

    var steamId = $("#steamIdTextBox").val();

    var ondone = function (data)
    {
        var gamesHtml = "\n";
        var steamId = $("#steamIdTextBox").val();

        // build the games list html
        $(data).each(function (index, game)
        {
            gamesHtml += "<div class='game'>";
            gamesHtml += "<a target='_blank' href='" + game.StoreUrl + "'><img src='" + game.ImageUrl + "' alt='" + game.Name + "' title='" + game.Name + "' /></a><br/>";
            gamesHtml += "<a target='_blank' href='" + game.StatsUrl + "?tab=achievements'>View Achievements</a>\n";
            gamesHtml += "</div>";
        });

        $("#gamesDiv").html(gamesHtml);

        log(gamesHtml);
        hideLoading(updatingSelector);
    };

    callAjax("GetGames", { steamUserId: steamId }, ondone);
}

function updateSteamUserId()
{
    if (!validateSteamUserId())
    {
        return false;
    }

    var updatingSelector = "#updatingSteamUserId";
    showLoading(updatingSelector);

    var steamUserId = $("#steamIdTextBox").val();
    var faceBookUserId = $("#facebookUserIdHidden").val();

    var ondone = function (data)
    {
        hideLoading(updatingSelector);
        showMessage("#steamIdUpdateSuccess");
    };

    var parameters = { "facebookUserId": faceBookUserId, "steamUserId": steamUserId };
    callAjax("UpdateSteamUserId", parameters, ondone);
}

function updateAchievements()
{
    $("#achievementsUpdateFailure").hide();

    if (!validateSteamUserId())
    {
        return false;
    }

    var updatingSelector = "#updatingAchievements";
    showLoading(updatingSelector);

    var steamUserId = $("#steamIdTextBox").val();

    var ondone = function (updateCount)
    {
        getGames();

        $("#newAchievementsLoading").show();

        callAjax("GetNewAchievements", { steamUserId: steamUserId },
            function (achievements)
            {
                hideLoading(updatingSelector);

                if (achievements.length == 0)
                {
                    showMessage("#noNewAchievementsMessage");
                }
                else
                {
                    if (achievements.length > 5)
                    {
                        // publish at most 5 achievements
                        achievements = achievements.slice(0, 5);
                    }

                    publishAchievements(achievements);
                }
            });
    };

    var onerror = function ()
    {
        hideLoading(updatingSelector);

        showMessage("#achievementsUpdateFailure");
    }

    var parameters = { "steamUserId": steamUserId };
    callAjax("UpdateAchievements", parameters, ondone, onerror);
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

    var steamUserId = $("#steamIdTextBox").val();

    var publishParams = {
        method: 'stream.publish',
        attachment: {
            name: steamUserId + " has earned new achievements",
            description: description,
            href: "http://steamcommunity.com/id/" + steamUserId,
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

            var data = { steamUserId: steamUserId, achievementIds: achievementIds };
            callAjax("PublishAchievements", data);
        }
    });
}

// validate steam user id
function validateSteamUserId()
{
    var $steamIdError = $("#steamIdError");
    $steamIdError.hide();

    var steamUserId = $("#steamIdTextBox").val();

    if (steamUserId == null || steamUserId == "")
    {
        $steamIdError.show();
        return false;
    }

    return true;
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

function showLoading(selector)
{
    $(selector).show();
}

function hideLoading(selector)
{
    $(selector).fadeOut("slow");
}

function showMessage(selector)
{
    $(selector).show("normal");
}

function log(message)
{
    if (_log)
    {
        $("#log").append(message);
    }
}