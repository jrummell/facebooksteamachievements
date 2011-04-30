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

    $("#saveButton").button({ icons: { primary: "ui-icon-disk"} })
                       .click(function ()
                       {
                           $("#saveImage").show();
                           $("#saveSuccess").hide();
                           $("#duplicateError").hide();

                           var settings = {
                               SteamUserId: $("#SteamUserId").val(),
                               AutoUpdate: $("#AutoUpdate")[0].checked,
                               PublishDescription: $("#PublishDescription")[0].checked
                           };

                           var ondone = function (status)
                           {
                               $("#saveImage").hide();

                               if (status == "Success")
                               {
                                   $("#saveSuccess").message({ type: "info" });
                               }
                               else if (status == "DuplicateError")
                               {
                                   $("#duplicateError").message({ type: "error" });
                               }
                           };

                           $achievements.saveSettings(settings, ondone);
                       });
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