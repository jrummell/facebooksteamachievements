var _serviceBase = "http://www.rummell.info/SteamAchievements/Services/Achievement.svc/";

function getGames()
{
    var ondone = function(data)
    {
        document.getElementById("debugDiv").setTextValue("ajax.ondone entry point");

        var games = data.payload.data;
        var gamesHtml = "<option>[select a game]</option>";

        for (var i = 0; i < games.length; i++)
        {
            var a = games[i];
            gamesHtml += "<option value='" + a.Id + "'>" + a.Name + "</option>";
        }

        document.getElementById("debugDiv").setTextValue("ajax.ondone done");
        document.getElementById("gamesSelect").setInnerXHTML(gamesHtml);
    };
    
    var onerror = function()
    {
        var gamesSelect = document.getElementById("gamesSelect");
        dialog = new Dialog(Dialog.DIALOG_CONTEXTUAL).setContext(gamesSelect).showMessage('Oops', 'A communication error occured. Try reloading the page.');
    };

    callAjax("GetGames", "{}", ondone, onerror);
}

function getAchievements()
{
    var ondone = function(data)
    {
        var achievementsHtml = "";

        for (var i = 0; i < data.length; i++)
        {
            var a = data[i];
            achievementsHtml += "<div>";
            achievementsHtml += "<div style='float: left;'><img src='" + a.ImageUrl + "' /></div>";
            achievementsHtml += "<div><strong>" + a.Name + "</strong><p>" + a.Description + "</p></div>";
            achievementsHtml += "</div>";
        }

        var achievementsDiv = document.getElementById("achievementsDiv");
        achievementsDiv.setInnerXHTML(achievementsHtml);
    };

    var onerror = function() { };

    var steamUserId = document.getElementById("steamIdTextBox").getValue();
    var gameId = document.getElementById("gamesSelect").getValue();

    var parameters = { "steamUserId": steamUserId, "gameId": gameId };
    callAjax("GetAchievements", parameters, ondone, onerror);
}

function updateSteamUserId()
{
    document.getElementById("steamIdError").setStyle("display", "none");

    var steamUserId = document.getElementById("steamIdTextBox").getValue();

    if (steamUserId == null || steamUserId == "")
    {
        document.getElementById("steamIdError").setStyle("display", "inline");
        return false;
    }

    //TODO: update achievements with web service
    var ondone = function(data) { /* display update success message */ };
    var onerror = function() { };

    var parameters = { "facebookUserId": null, "steamUserId": steamUserId };
    callAjax("UpdateSteamId", parameters, ondone, onerror);
}

function callAjax(method, query, ondone, onerror)
{
    var ajax = new Ajax();
    ajax.responseType = Ajax.JSON;
    //ajax.requireLogin = true;
    ajax.useLocalProxy = true;
    ajax.ondone = ondone;
    ajax.onerror = onerror;
    
    ajax.post(_serviceBase + method, query);
}