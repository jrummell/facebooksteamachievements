import AchievementService from "./AchievementService";

class GamesPage {
    constructor() {
        var steamUserId = $("#SteamUserId").val();
        var enableLog = $("#EnableLog").val() == "True";

        this.achievementService = new AchievementService(steamUserId, enableLog, false);
    }

    getProfile(callback) {
        this.achievementService.loadProfile("#profileDiv",
            error => {
                if (error) {
                    return;
                }
                if (typeof(callback) == "function") {
                    callback();
                }
                this.achievementService.updateSize();
            });
    }

    getGames()
    {
        $("#gamesContainer").show();
        var updatingSelector = "#loadingGames";
        this.achievementService.showLoading(updatingSelector);

        this.achievementService.loadGames("#gamesDiv",
            () => {
                this.achievementService.hideLoading(updatingSelector);

                this.achievementService.updateSize();
            });
    }

    load() {
        if ($(".games-page").length === 0) {
            return;
        }

        this.getProfile(() => this.getGames());
    }
}

$(document).ready(function () {
    var games = new GamesPage();
    games.load();
});