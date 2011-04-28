/// <reference path="jquery-vsdoc.js" />
/// <reference path="jquery.message.js" />
/// <reference path="achievements.js" />

$(document).ready(function ()
{
    var logSelector = "#log";
    $steamUserId = $("#SteamUserId");
    $achievements.init($steamUserId.val(), logSelector, false);

    $steamUserId.change(function ()
    {
        checkProfile(this.value);
    });

    $("#steamIdError").message({ type: "error", dismiss: false }).hide();
});

function checkProfile(steamUserId)
{
    $("#steamIdError").hide();
    $("#steamIdVerified").hide();
    var ondone = function (profile)
    {
        if (profile == null)
        {
            $("#steamIdError").show();
            return;
        }
        else
        {
            $("#steamIdVerified").show();
        }
    };

    $achievements.getProfile(ondone, null, steamUserId);
}