/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.ui.message.js" />
/// <reference path="achievements.js" />

$(document).ready(function ()
{
    var logSelector = "#log";
    var $steamUserId = $("#SteamUserId");
    $achievements.init($steamUserId.val(), logSelector, false);

    // check the user's profile when they change it
    $steamUserId.change(function ()
    {
        checkProfile();
    });

    // check the user's profile on load
    checkProfile();

    // init save success message
    $("#saveSuccess").message({ type: "info" });

    // show loading on click
    $("#saveButton").button({ icons: { primary: "ui-icon-disk"} })
        .click(function ()
        {
            $("#saveImage").show();
            $("#saveSuccess").hide();

            return true;
        });
});

function checkProfile()
{
    var steamUserId = $("#SteamUserId").val();
    
    $("#steamIdError").hide();
    $("#steamIdVerified").hide();

    var ondone = function (valid)
    {
        if (!valid)
        {
            $("#steamIdError").show();
            return;
        }
        else
        {
            $("#steamIdVerified .profile-url").attr("href", "http://steamcommunity.com/id/" + steamUserId);
            $("#steamIdVerified").show();
        }
    };

    $achievements.validateProfile(steamUserId, ondone);
}