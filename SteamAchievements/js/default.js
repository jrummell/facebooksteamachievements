/// <reference path="jquery-vsdoc.js" />
/// <reference path="achievements.js" />

$(document).ready(function ()
{
    var steamUserId = $("#steamUserIdHidden").val();
    var logSelector = "#log";
    $achievements.init(steamUserId, logSelector, false);

    var valid = $achievements.validateSteamUserId("#steamIdError");
    if (!valid)
    {
        return;
    }

    $("#steamUserIdHeading").text(steamUserId);

    getGames();
});

function getGames()
{
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