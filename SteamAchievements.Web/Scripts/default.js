/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.ui.message.js" />
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

    validateProfile();

    getProfile();

    getGames();
});

function getProfile()
{
    var ondone = function ()
    {
        $achievements.updateSize();
    };

    $achievements.loadProfile("#profileDiv", ondone);
}

function validateProfile()
{
    var ondone = function (valid)
    {
        if (!valid)
        {
            $("#steamIdError").message({ type: "error", dismiss: false });
            return;
        }
    };

    $achievements.validateProfile(null, ondone);
}

function getGames()
{
    var updatingSelector = "#loadingGames";
    $achievements.showLoading(updatingSelector);

    var ondone = function ()
    {
        $("#gamesDiv ul").makeacolumnlists({ cols: 3, equalHeight: "ul" });

        $achievements.hideLoading(updatingSelector);

        $achievements.updateSize();
    };

    $achievements.loadGames("#gamesDiv", ondone);
}