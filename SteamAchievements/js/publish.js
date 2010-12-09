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
            $achievements.publishAchievements(achievementsToPublish, function ()
            {
                getNewAchievements();
            });
        }
    });
});

function getNewAchievements()
{
    $achievements.showLoading("#newAchievementsLoading");

    $achievements.updateAchievements(displayAchievements, displayError);
}

var _newAchievements = new Array();
function displayAchievements(achievements)
{
    _newAchievements = new Array();
    var achievementHtml = "\n";
    var gameId = new String();
    $(achievements).each(function (i)
    {
        var achievement = achievements[i];
        _newAchievements.push(achievement);

        if (gameId != achievement.Game.Id)
        {
            achievementHtml += "<h3>" + achievement.Game.Name + "\</h3>";
            gameId = achievement.Game.Id;
        }

        achievementHtml += "<div class='achievement'><input value='" + achievement.Id + "' type='checkbox' \/>";
        achievementHtml += "<img src='" + achievement.ImageUrl + "' alt='" + achievement.Description + "' \/>";
        achievementHtml += "<span class='description'>" + achievement.Description + "<\/span><\/div>\n";
    });

    $("#newAchievements").html(achievementHtml);

    // allow user to select only 5 achievements since only 5 images can be displayed at a time
    $("#newAchievements .achievement :checkbox, #newAchievements .achievement img").click(function ()
    {
        $(this).parents(".achievement").toggleClass("selected");
        if (this.tagName == "IMG")
        {
            var checkbox = $(this).prev()[0];
            checkbox.checked = !checkbox.checked;
        }

        var $checked = $("#newAchievements :checked");
        var disableUnchecked = $checked.length >= 5;
        $("#newAchievements .achievement :checkbox").filter(function (index)
        {
            return !this.checked;
        }).attr("disabled", disableUnchecked);
    });

    if (achievements.length == 0)
    {
        $("#noUnpublishedMessage").show();
    }
    else
    {
        $("#publishSelectedButton").show();
    }

    $achievements.hideLoading("#newAchievementsLoading");
}

function displayError()
{
    $achievements.showMessage("#achievementsUpdateFailure");
}