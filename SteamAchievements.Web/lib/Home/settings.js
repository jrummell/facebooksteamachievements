/// <reference path="~/Scripts/Shared/AchievementService.js"/>

$(document).ready(function () {
    var $steamUserId = $("#SteamUserId");
    var enableLog = $("#EnableLog").val() === "True";
    var achievementService = new AchievementService($steamUserId.val(), enableLog, false);

    // check the user's profile when they change it
    $steamUserId.change(function () {
        checkProfile();
    });

    // check the user's profile on load
    checkProfile();

    // init save success message
    $("#saveSuccess").message({ type: "info" });

    // show loading on click
    var $saveButton = $("#saveButton");
    $saveButton.click(function () {
        achievementService.showLoading("#saveImage");
        $("#saveSuccess").hide();

        return true;
    });

    function checkProfile() {
        var steamUserId = $steamUserId.val();

        $("#steamIdError").hide();
        $("#steamIdVerified").hide();

        var ondone = function (data) {
            $(".settings .profile-url").attr("href", "http://steamcommunity.com/id/" + steamUserId);

            if (!data.Valid) {
                $("#steamIdError").show();
                return;
            } else {
                $("#steamIdVerified").show();
            }
        };

        achievementService.validateProfile(steamUserId, "#validateProfileError", ondone);
    }
});