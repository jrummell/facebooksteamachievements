/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.ui.message.js" />
/// <reference path="achievements.js" />
/// <reference path="columnizer.js" />

$(document).ready(function() {
    var steamUserId = $("#SteamUserId").val();
    var signedRequest = $("#SignedRequest").val();
    var enableLog = $("#EnableLog").val() == "True";
    var logOnRedirectUrl = $("#LogOnRedirectUrl").val();

    var achievementService = new AchievementService(steamUserId, signedRequest, enableLog, false);

    if (logOnRedirectUrl.length > 1)
    {
        var hasAccessToken = achievementService.updateAccessTokenFromHash(function() {
            window.reload();
        });

        if (!hasAccessToken)
        {
            top.location.href = logOnRedirectUrl;
        }
    }

    var valid = achievementService.validateSteamUserId("#steamIdError");
    if (!valid)
    {
        return;
    }

    validateProfile();

    function validateProfile()
    {
        /// <summary>validates profile and then loads profile and games</summary>
        var ondone = function(valid)
        {
            if (!valid)
            {
                $("#steamIdError").message({ type: "error", dismiss: false });
                return;
            }

            getProfile();
            getGames();

            achievementService.updateAccessToken();
        };

        achievementService.validateProfile(null, ondone);
    }

    function getProfile()
    {
        var ondone = function()
        {
            achievementService.updateSize();
        };

        achievementService.loadProfile("#profileDiv", ondone);
    }

    function getGames()
    {
        $("#gamesContainer").show();
        var updatingSelector = "#loadingGames";
        achievementService.showLoading(updatingSelector);

        var ondone = function()
        {
            if (achievementService.mobile)
            {
                $("#gamesDiv ul").attr("data-inset", "true").listview();
            }
            else if ($.fn.makeacolumnlists)
            {
                $("#gamesDiv ul").makeacolumnlists({ cols: 3, equalHeight: "ul" });
            }

            achievementService.hideLoading(updatingSelector);

            achievementService.updateSize();
        };

        achievementService.loadGames("#gamesDiv", ondone);
    }
});