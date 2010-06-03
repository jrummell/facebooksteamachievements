/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />
/// <reference path="js/FeatureLoader.js" />
var _serviceBase = "Services/Achievement.svc/";
var _log = false;

$(document).ready(function()
{
    init();
    getGames();
});

function init()
{
    $("img.loading").hide();

    $("div.message").append(" <span class='dismiss'>Click to dismiss.</span>").click(function()
    {
        $(this).hide('normal');
    });

    $("#updateSteamIdButton").click(function()
    {
        var result = updateSteamUserId();

        if (result == false)
        {
            return false;
        }

        getGames();
    });

    $("#updateAchievementsButton").click(function()
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

    var ondone = function(data)
    {
        var gamesHtml = "\n";
        var steamId = $("#steamIdTextBox").val();

        $(data).each(function(index, game)
        {
            var url = "http://steamcommunity.com/id/" + steamId + "/stats/" + game.Abbreviation + "/";
            var aStart = "<a target='_blank' href='" + url + "'>";
            gamesHtml += "<div class='game'>";
            gamesHtml += aStart + "<img src='" + game.ImageUrl + "' alt='" + game.Name + "' title='" + game.Name + "' /></a>";
            gamesHtml += aStart + "View Achievements</a>\n";
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

    var ondone = function(data)
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

    var ondone = function(data)
    {
        hideLoading(updatingSelector);

        getGames();

        publishLatestAchievements(steamUserId);

        // set the new achievement count
        $("#newAchievementCount").text(data);

        showMessage("#achievementsUpdateSuccess");
    };

    var onerror = function()
    {
        hideLoading(updatingSelector);

        showMessage("#achievementsUpdateFailure");
    }

    var parameters = { "steamUserId": steamUserId };
    callAjax("UpdateAchievements", parameters, ondone, onerror);
}

function publishLatestAchievements(steamUserId)
{
    var faceBookUserId = $("#facebookUserIdHidden").val();
    var parameters = { "steamUserId": steamUserId, "facebookUserId": faceBookUserId };
    callAjax("PublishLatestAchievements", parameters, null);
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
        onerror = function(m)
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
        error: function(xhr)
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