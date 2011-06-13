/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.message.js" />
/// <reference path="achievements.js" />
/// <reference path="columnizer.js" />

$(document).ready(function ()
{
    var steamUserId = $("#steamUserIdHidden").val();
    var logSelector = "#log";
    var enableLog = $("#enableLogHidden").val() == "True";
    var publishDescription = $("#publishDescriptionHidden").val() == "True";
    $achievements.init(steamUserId, logSelector, enableLog, publishDescription);

    var valid = $achievements.validateSteamUserId("#steamIdError");
    if (!valid)
    {
        return;
    }

    getNewAchievements();

    $("#publishSelectedButton").click(function ()
    {
        var achievementsToPublish = new Array();
        $("#newAchievements input:checked").each(function ()
        {
            var achievementId = this.value;
            var matched = $.grep(_newAchievements, function (achievement, i)
            {
                return achievement.Id == achievementId;
            });

            $.each(matched, function (i, achievement)
            {
                achievementsToPublish.push(achievement);
            });
        });

        if (achievementsToPublish.length > 0)
        {
            $achievements.publishAchievements(achievementsToPublish, getNewAchievements);
        }
    });

    $("#hideSelectedButton").click(function ()
    {
        var achievementsToHide = new Array();
        $("#newAchievements input:checked").each(function ()
        {
            achievementsToHide.push(this.value);
        });

        if (achievementsToHide.length > 0)
        {
            $achievements.hideAchievements(achievementsToHide, getNewAchievements);
        }
    });
});

function getNewAchievements()
{
    // remove any currently displayed achievements
    $("#newAchievements").empty();

    $achievements.showLoading("#newAchievementsLoading");

    $achievements.updateAchievements(displayAchievements, displayError);
}

var _newAchievements = new Array();
function displayAchievements(achievements)
{
    _newAchievements = achievements;

    $("#newAchievements").empty().append("<ul></ul>");
    $("#achievementTemplate").tmpl(achievements).appendTo("#newAchievements ul");

    // allow user to select only 5 achievements since only 5 images can be displayed at a time
    $("#newAchievements .achievement :checkbox, #newAchievements .achievement img").click(function ()
    {
        var $checked = $("#newAchievements :checked");
        var disableUnchecked = $checked.length >= 5;

        // toggle the selection if there are less than 5 selected or if the current item is already selected
        var $achievementDiv = $(this).parents(".achievement");
        if (!disableUnchecked || $achievementDiv.hasClass("selected"))
        {
            $achievementDiv.toggleClass("selected");
            if (this.tagName == "IMG")
            {
                var checkbox = $(this).prev()[0];
                checkbox.checked = !checkbox.checked;
            }
        }

        // disable unchecked boxes if there are 5 checked
        $("#newAchievements .achievement :checkbox").filter(function (index)
        {
            return !this.checked;
        }).attr("disabled", disableUnchecked);
    });

    $("#newAchievements ul").makeacolumnlists({ cols: 2, equalHeight: "ul" });

    if (achievements.length == 0)
    {
        $("#noUnpublishedMessage").message({ type: "info" });
    }
    else
    {
        $("#buttons").show();
    }

    $achievements.hideLoading("#newAchievementsLoading");
}

function displayError()
{
    $achievements.hideLoading("#newAchievementsLoading");
    $("#achievementsUpdateFailure").message({ type: "error", dismiss: false });
}