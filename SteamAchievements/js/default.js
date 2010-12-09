/// <reference path="jquery-vsdoc.js" />
/// <reference path="achievements.js" />

$(document).ready(function ()
{
    init();
    getGames();
});

function init()
{
    var steamUserId = $("#steamUserIdHidden").val();
    var logSelector = "#log";
    $achievements.init(steamUserId, logSelector, false);

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
    $achievements.showLoading(updatingSelector);

    var ondone = function (games)
    {
        var gamesHtml = "\n";

        // build the games list html
        $(games).each(function (index, game)
        {
            gamesHtml += "<div class='game'>";
            gamesHtml += "<a target='_blank' href='" + game.StoreUrl + "'><img src='" + game.ImageUrl + "' alt='" + game.Name + "' title='" + game.Name + "' /></a><br/>";
            gamesHtml += "<a target='_blank' href='" + game.StatsUrl + "?tab=achievements'>View Achievements</a>\n";
            gamesHtml += "</div>";
        });

        $("#gamesDiv").html(gamesHtml);

        $achievements.log(gamesHtml);
        $achievements.hideLoading(updatingSelector);

        $achievements.updateSize();
    };

    $achievements.getGames(ondone);
}

function updateAchievements()
{
    $("#achievementsUpdateFailure").hide();

    if (!validateSteamUserId())
    {
        return false;
    }

    var updatingSelector = "#updatingAchievements";
    $achievements.showLoading(updatingSelector);

    var ondone = function (achievements)
    {
        getGames();

        $achievements.showLoading("#newAchievementsLoading");

        $achievements.hideLoading(updatingSelector);

        if (achievements.length == 0)
        {
            $achievements.showMessage("#noNewAchievementsMessage");
        }
        else
        {
            if (achievements.length > 5)
            {
                // publish at most 5 achievements
                achievements = achievements.slice(0, 5);
            }

            $achievements.publishAchievements(achievements);
        }
    };

    var onerror = function ()
    {
        $achievements.hideLoading(updatingSelector);

        $achievements.showMessage("#achievementsUpdateFailure");
    }

    $achievements.updateAchievements(ondone, onerror);
}

// validate steam user id
function validateSteamUserId()
{
    var $steamIdError = $("#steamIdError");
    $steamIdError.hide();

    if ($achievements.steamUserId == null || $achievements.steamUserId == "")
    {
        $steamIdError.show();
        return false;
    }

    return true;
}