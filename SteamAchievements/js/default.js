/// <reference path="jquery-vsdoc.js" />
/// <reference path="achievements.js" />
/// <reference path="columnizer.js" />

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
        $("#gamesDiv").empty().append("<ul></ul>");
        $("#gamesTemplate").tmpl(achievements).appendTo("#gamesDiv ul");

        $("#gamesDiv ul").makeacolumnlists({ cols: 2, equalHeight: "ul" });

        $achievements.log(gamesHtml);
        $achievements.hideLoading(updatingSelector);

        $achievements.updateSize();
    };

    $achievements.getGames(ondone);
}