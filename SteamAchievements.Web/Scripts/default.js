/// <reference path="jquery-vsdoc.js" />
/// <reference path="jquery.message.js" />
/// <reference path="achievements.js" />
/// <reference path="columnizer.js" />

$(document).ready(function ()
{
    $("#publishPageMessage").message({ type: "info" });

    var steamUserId = $("#steamUserIdHidden").val();
    var logSelector = "#log";
    $achievements.init(steamUserId, logSelector, false);

    var valid = $achievements.validateSteamUserId("#steamIdError");
    if (!valid)
    {
        return;
    }

    getProfile();

    getGames();
});

function getProfile()
{
    var ondone = function (profile)
    {
        if (profile == null)
        {
            $("#steamIdError").message({ type: "error", dismiss: false });
            return;
        }
        $("#profileImage").attr("src", profile.AvatarUrl);
        $("#headlineLabel").text(profile.Headline);
        $("#steamUserIdHeading").text(profile.SteamUserId);

        $("#heading").show();

        $achievements.updateSize();
    };

    $achievements.getProfile(ondone);
}

function getGames()
{
    var updatingSelector = "#updatingGames";
    $achievements.showLoading(updatingSelector);

    var ondone = function (games)
    {
        $("#gamesDiv").empty().append("<ul></ul>");
        $("#gamesTemplate").tmpl(games).appendTo("#gamesDiv ul");

        $("#gamesDiv ul").makeacolumnlists({ cols: 3, equalHeight: "ul" });

        $achievements.hideLoading(updatingSelector);

        $achievements.updateSize();
    };

    $achievements.getGames(ondone);
}