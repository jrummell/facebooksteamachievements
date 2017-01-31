import AchievementService from "./AchievementService";

class SettingsPage {
    constructor() {
        this.$steamUserId = $("#SteamUserId");
        this.enableLog = $("#EnableLog").val() === "True";
        this.achievementService = new AchievementService(this.$steamUserId.val(), this.enableLog, false);
    }

    checkProfile()
    {
        var steamUserId = this.$steamUserId.val();

        $("#steamIdError").hide();
        $("#steamIdVerified").hide();

        var ondone = function(data)
        {
            $(".settings .profile-url").attr("href", "http://steamcommunity.com/id/" + steamUserId);

            if (!data.Valid)
            {
                $("#steamIdError").show();
                return;
            }
            else
            {
                $("#steamIdVerified").show();
            }
        };

        this.achievementService.validateProfile(steamUserId, "#validateProfileError", ondone);
    }

    load() {
        var self = this;
        if ($(".settings-page").length === 0) {
            return;
        }

        // check the user's profile when they change it
        this.$steamUserId.change(function()
        {
            self.checkProfile();
        });

        // check the user's profile on load
        self.checkProfile();

        // init save success message
        $("#saveSuccess").message({ type: "info" });

        // show loading on click
        var $saveButton = $("#saveButton");
        $saveButton.click(function()
        {
            self.achievementService.showLoading("#saveImage");
            $("#saveSuccess").hide();

            return true;
        });
    }
}

$(document).ready(function () {
    var settings = new SettingsPage();
    settings.load();
});