/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />
/// <reference path="js/FeatureLoader.js" />
var _serviceBase = "Services/Achievement.svc/";
var _log = false;

function getGames()
{
    var ondone = function(data)
    {
        var games = data;
        var gamesHtml = "<option value=''>[select a game]</option>";

        for (var i = 0; i < games.length; i++)
        {
            var a = games[i];
            gamesHtml += "<option value='" + a.Id + "'>" + a.Name + "</option>";
        }

        log(gamesHtml);
        $("#gamesSelect").html(gamesHtml);
    };

    callAjax("GetGames", {}, ondone);
}

function updateSteamUserId()
{
    if (!validateSteamUserId())
    {
        return false;
    }

    var $updating = $("#updatingSteamUserId");
    $updating.show();

    var steamUserId = $("#steamIdTextBox").val();
    var faceBookUserId = $("#facebookUserIdHidden").val();

    var ondone = function(data)
    {
        $updating.hide();
    };

    var parameters = { "FacebookUserId": faceBookUserId, "SteamUserId": steamUserId };
    callAjax("UpdateSteamUserId", parameters, ondone);
}

function getAchievements()
{
    if (!validateSteamUserId())
    {
        return false;
    }

    var steamUserId = $("#steamIdTextBox").val();
    var gameId = $("#gamesSelect").val();

    if (gameId == null || gameId == "")
    {
        return false;
    }

    var $achievements = $("#achievementsDiv");
    var $loading = $("#loadingAchievements");
    $loading.show();

    var ondone = function(data)
    {
        var achievementsHtml = "";

        for (var i = 0; i < data.length; i++)
        {
            var a = data[i];
            achievementsHtml += "<div class='achievement'>";
            achievementsHtml += "<div style='float: left;'><img src='" + a.ImageUrl + "' /></div>";
            achievementsHtml += "<div><h3>" + a.Name + "</h3><p>" + a.Description + "</p></div>";
            achievementsHtml += "<br class='clear'/></div>";
        }

        if (achievementsHtml == "")
        {
            achievementsHtml = "You haven't earned any achievements for this game yet!";
        }

        log(achievementsHtml);
        $achievements.html(achievementsHtml);
        $loading.hide();
    };

    var parameters = { "SteamUserId": steamUserId, "GameId": gameId };
    callAjax("GetAchievements", parameters, ondone);
}

function updateAchievements()
{
    if (!validateSteamUserId())
    {
        return false;
    }

    var $updating = $("#updatingAchievements");
    $updating.show();

    var steamUserId = $("#steamIdTextBox").val();

    var ondone = function(data)
    {
        $updating.hide();

        getAchievements();

        publishLatestAchievements(steamUserId);
    };

    var parameters = { "SteamUserId": steamUserId };
    callAjax("UpdateAchievements", parameters, ondone);
}

function publishLatestAchievements(steamUserId)
{
    var faceBookUserId = $("#facebookUserIdHidden").val();
    var parameters = { "SteamUserId": steamUserId, "FacebookUserId": faceBookUserId };
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

function callAjax(method, query, ondone)
{
    var onerror = function(m)
    {
        $("#log").text(m.Message).show();
    };

    $.ajax({
        url: _serviceBase + method,
        data: JSON.stringify(query),
        type: "POST",
        processData: true,
        contentType: "application/json",
        timeout: 10000,
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
                    onerror({ Message: e.toString() });
                }
            }
            return;
        }
    });
}

function log(message)
{
    if (_log)
    {
        $("#log").append(message);
    }
}