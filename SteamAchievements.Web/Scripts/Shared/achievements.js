/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery.ui.message.js" />
/// <reference path="json2.js" />
/// <reference path="columnizer.js" />

if (!window.console)
{
    window.console = { };
    if (!window.console.log)
    {
        window.console.log = function()
        {
        };
    }
}

function AchievementService(steamUserId, signedRequest, enableLog, publishDescription)
{
    var self = this;

    // fields
    self.mobile = typeof($.mobile) != "undefined";
    self.steamUserId = steamUserId;
    self.signedRequest = signedRequest;
    self.serviceBase = "Achievement/";
    self.enableLog = enableLog === true;
    self.publishDescription = publishDescription;

    // methods
    self.loadProfile = function(selector, callback)
    {
        load(selector, "Profile", { steamUserId: self.steamUserId }, callback);
    };

    self.validateProfile = function(steamUserIdToValidate, callback)
    {
        steamUserIdToValidate = steamUserIdToValidate || self.steamUserId;
        var data;
        if (steamUserIdToValidate)
        {
            data = { steamUserId: steamUserIdToValidate };
        }
        else
        {
            data = { };
        }

        post("ValidateProfile", data, callback);
    };

    self.updateAccessToken = function(callback)
    {
        post("UpdateAccessToken", { }, callback);
    };

    self.loadGames = function(selector, callback)
    {
        load(selector, "Games", { steamUserId: self.steamUserId }, callback);
    };

    self.updateAchievements = function(callback, errorCallback)
    {
        post("UpdateAchievements", { }, callback, errorCallback);
    };

    self.loadUnpublishedAchievements = function(selector, callback)
    {
        load(selector, "UnpublishedAchievements", { }, callback);
    };

    self.hideAchievements = function(achievementIds, callback, errorCallback)
    {
        var parameters = { achievementIds: achievementIds };
        post("HideAchievements", parameters, callback, errorCallback);
    };

    self.publishAchievements = function(achievements, callback, errorCallback)
    {
        // display publish dialog

        var image = null;
        var description = new String();
        var gameId = new String();

        $.each(achievements, function(i) {
            var achievement = this; // achievements[i];

            if (image == null)
            {
                image = achievement.ImageUrl;
            }

            if (gameId != achievement.Game.Id)
            {
                gameId = achievement.Game.Id;

                if (i > 0 && description.length > 2)
                {
                    // replace last comma with period
                    description = description.substring(0, description.length - 2);
                    description += ". ";
                }

                description += achievement.Game.Name + ": ";
            }

            description += achievement.Name;

            if (self.publishDescription)
            {
                description += " (" + achievement.Description + ")";
            }

            if (i < achievements.length - 1)
            {
                description += ", ";
            }
            else
            {
                description += ".";
            }

            self.log(description);
        });

        var message = self.steamUserId + " unlocked " + achievements.length + " achievement" + (achievements.length > 1 ? "s" : "") + "!";
        var publishParams = {
            method: "feed",
            link: "http://steamcommunity.com/id/" + self.steamUserId,
            picture: image,
            message: message, // message seems to be broken
            name: message,
            description: description
        };

        // create and anchor in the middle of the page and focus on it so that the dialog will be visible to the user.
        var $middleAnchor = $("#middleAnchor");
        if ($middleAnchor.length == 0)
        {
            // add an anchor in the middle of the page
            var middleX = $(document).width() / 2;
            var middleY = $(document).height() / 2;

            // Chrome requires that the anchor contains text so that focus will work
            $("body").append("<a id='middleAnchor' href='#'>.<\/a>");
            $middleAnchor = $("#middleAnchor");
            $middleAnchor.css("left", middleX).css("top", middleY);
        }

        $middleAnchor.focus();

        FB.ui(publishParams, function(response) {
            if (response && response.post_id)
            {
                // on successful publish, update published field on each published achievement.

                var achievementIds = new Array();
                for (var i = 0; i < achievements.length; i++)
                {
                    achievementIds.push(achievements[i].Id);
                }

                var data = { achievementIds: achievementIds };
                post("PublishAchievements", data, callback, errorCallback);
            }
        });
    };

    self.validateSteamUserId = function(errorMessageSelector)
    {
        var valid = true;
        if (self.steamUserId == null || self.steamUserId == "")
        {
            valid = false;
        }

        if (!valid)
        {
            $(errorMessageSelector).message({ type: "error", dismiss: false });
        }

        return valid;
    };

    self.showLoading = function(selector)
    {
        if (self.mobile)
        {
            $.mobile.showPageLoadingMsg();
        }
        else
        {
            $(selector).show("normal", self.updateSize);
        }
    };

    self.hideLoading = function(selector)
    {
        if (self.mobile)
        {
            $.mobile.hidePageLoadingMsg();
        }
        else
        {
            $(selector || "img.loading").fadeOut("slow", self.updateSize);
        }
    };

    self.updateSize = function()
    {
        if (self.mobile)
        {
            return;
        }

        if (typeof(FB) != "undefined")
        {
            //TODO: the canvas resize api has changed
            // update the size of the iframe to match the content
            FB.Canvas.setSize();
        }
    };

    self.log = function(message)
    {
        if (self.enableLog)
        {
            console.log(message);
        }
    };

    // private methods

    function load(selector, method, params, ondone)
    {
        if (params == null)
        {
            params = { };
        }
        setSignedRequest(params);

        var url = self.serviceBase + method;
        $(selector).load(url, params, ondone);
    }

    function post(method, params, ondone, onerror)
    {
        if (onerror == null)
        {
            onerror = function(m)
            {
                self.log(m.Message);
            };
        }

        if (params == null)
        {
            params = { };
        }
        setSignedRequest(params);

        $.ajax({
            url: self.serviceBase + method,
            data: JSON.stringify(params),
            type: "POST",
            processData: true,
            contentType: "application/json",
            timeout: 120000, // 2 minutes
            dataType: "json",
            success: ondone,
            error: function(xhr)
            {
                if (!onerror)
                {
                    return;
                }

                if (xhr.responseText)
                {
                    try
                    {
                        var err = JSON.parse(xhr.responseText);
                        if (err)
                        {
                            onerror(err);
                        }
                        else
                        {
                            onerror({ Message: "Unknown server error." });
                        }
                    }
                    catch(e)
                    {
                        onerror({ Message: "Unknown server error." });
                    }
                }
                return;
            }
        });
    }

    function setSignedRequest(params)
    {
        // since this is an ajax request, we need to add the signed_request parameter explicitly
        params.signed_request = self.signedRequest;
    }

    // init
    self.hideLoading("img.loading");
}