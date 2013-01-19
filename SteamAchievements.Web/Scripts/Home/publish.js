// achievement array set by the Unpublished Achievements view (loaded via ajax)
var _newAchievements = new Array();

$(document).ready(function()
{
    var steamUserId = $("#SteamUserId").val();
    var publishDescription = $("#PublishDescription").val() === "True";
    var signedRequest = $("#SignedRequest").val();
    var enableLog = $("#EnableLog").val() === "True";
    var achievementService = new AchievementService(steamUserId, signedRequest, enableLog, publishDescription);

    var valid = achievementService.validateSteamUserId("#steamIdError");
    if (!valid)
    {
        return;
    }

    var validateProfileCallback = function(data)
    {
        if (!data.Valid)
        {
            return;
        }

        getNewAchievements();
    };
    achievementService.validateProfile(steamUserId, "#validateProfileError", validateProfileCallback);

    $("#publishSelectedButton").click(function()
    {
        var achievementsToPublish = new Array();
        $("#newAchievements input:checked").each(function()
        {
            var achievementId = this.value;
            var matched = $.grep(_newAchievements, function(achievement, i)
            {
                return achievement.Id == achievementId;
            });

            $.each(matched, function(i, achievement)
            {
                achievementsToPublish.push(achievement);
            });
        });

        if (achievementsToPublish.length > 0)
        {
            achievementService.publishAchievements(achievementsToPublish, getNewAchievements);
        }
    });

    $("#hideSelectedButton").click(function()
    {
        var confirmed = confirm("Are you sure you want to hide the selected achievements?");
        if (!confirmed)
        {
            return false;
        }

        var achievementsToHide = new Array();
        $("#newAchievements input:checked").each(function()
        {
            achievementsToHide.push(this.value);
        });

        if (achievementsToHide.length > 0)
        {
            achievementService.hideAchievements(achievementsToHide, getNewAchievements);
        }
    });

    function getNewAchievements()
    {
        // remove any currently displayed achievements
        $("#newAchievements").empty();

        achievementService.showLoading("#newAchievementsLoading");

        achievementService.updateAchievements(function(data)
        {
            if (data && data.Error)
            {
                displayError(data.Error);
            }
            else
            {
                displayAchievements();
            }
        }, displayError);
    }

    function displayAchievements()
    {
        var ondone = function()
        {
            // allow user to select only 5 achievements since only 5 images can be displayed at a time
            $("#newAchievements .achievement :checkbox, #newAchievements .achievement img").click(function()
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
                        var checkbox = $achievementDiv.find(":checkbox").get(0);
                        checkbox.checked = !checkbox.checked;

                        if (achievementService.mobile)
                        {
                            checkbox.checkboxradio("refresh");
                        }
                    }
                }

                // disable unchecked boxes if there are 5 checked
                $("#newAchievements .achievement :checkbox").filter(function()
                {
                    return !this.checked;
                }).attr("disabled", disableUnchecked);
            });

            if (achievementService.mobile)
            {
                $("#newAchievements .achievement :checkbox").checkboxradio();
                $("#newAchievements ul").attr("data-inset", "true").listview();
            }

            if (_newAchievements.length == 0)
            {
                $("#noUnpublishedMessage").message({ type: "info" });
            }
            else
            {
                $("#buttons").show();
            }

            achievementService.hideLoading("#newAchievementsLoading");
        };

        achievementService.loadUnpublishedAchievements("#newAchievements", ondone);
    }

    function displayError(error)
    {
        achievementService.hideLoading("#newAchievementsLoading");

        var $error = $("#achievementsUpdateFailure");
        var $errorMessage = $(".error-message", $error);
        $errorMessage.hide().text("");

        var options = { type: "error", dismiss: false };
        if (error)
        {
            var errorMessage = error.Message || error;

            $errorMessage.show().text("Additional information: " + errorMessage);
            if (error.StackTrace)
            {
                $error.after("<pre class='error-stacktrace hidden'>" + error.StackTrace + "</pre>");
            }
        }

        $error.message(options);
    }
});