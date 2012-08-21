$(document).ready(function () {
    var $steamUserId = $("#SteamUserId");
    var signedRequest = $("#SignedRequest").val();
    var enableLog = $("#EnableLog").val() == "True";
    var achievementService = new AchievementService($steamUserId.val(), signedRequest, enableLog, false);

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
    if (!achievementService.mobile) {
        $saveButton.button({ icons: { primary: "ui-icon-disk"} });
    }
    $saveButton.click(function () {
        achievementService.showLoading("#saveImage");
        $("#saveSuccess").hide();

        return true;
    });

    function checkProfile() {
        var steamUserId = $steamUserId.val();

        $("#steamIdError").hide();
        $("#steamIdVerified").hide();

        var ondone = function (valid) {
            $(".settings .profile-url").attr("href", "http://steamcommunity.com/id/" + steamUserId);

            if (!valid) {
                $("#steamIdError").show();
                return;
            }
            else {
                $("#steamIdVerified").show();
            }
        };

        achievementService.validateProfile(steamUserId, ondone);
    }
});