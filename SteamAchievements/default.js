/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />
/// <reference path="js/FeatureLoader.js" />
var _serviceBase = "Services/Achievement.svc/";
var _log = false;

function init()
{
    $("img.loading").hide();
}

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
    var loadingSelector = "#loadingAchievements";
    showLoading(loadingSelector);

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
        hideLoading(loadingSelector);
    };

    var parameters = { "steamUserId": steamUserId, "gameId": gameId };
    callAjax("GetAchievements", parameters, ondone);
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

        getAchievements();

        publishLatestAchievements(steamUserId);

        showMessage("#achievementsUpdateSuccess");
    };

    var onerror = function()
    {
        hideLoading(updatingSelector);
        
        showMessage("#achievementsUpdateFailure", false);
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

function showMessage(selector, fadeOut)
{
    if (fadeOut == null)
    {
        fadeOut = true;
    }

    $(selector).show("normal");

    if (fadeOut)
    {
        setTimeout("$('" + selector + "').hide('slow');", 5000);
    }
}

function log(message)
{
    if (_log)
    {
        $("#log").append(message);
    }
}