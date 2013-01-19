$(document).ready(function()
{
    var steamUserId = $("#SteamUserId").val();
    var signedRequest = $("#SignedRequest").val();
    var enableLog = $("#EnableLog").val() == "True";

    var achievementService = new AchievementService(steamUserId, signedRequest, enableLog, false);

    getProfile(getGames);

    function getProfile(callback)
    {
        var ondone = function(error)
        {
            if (error)
            {
                return;
            }
            if (typeof(callback) == "function")
            {
                callback();
            }
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

            achievementService.hideLoading(updatingSelector);

            achievementService.updateSize();
        };

        achievementService.loadGames("#gamesDiv", ondone);
    }
});