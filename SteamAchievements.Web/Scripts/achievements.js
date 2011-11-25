/// <reference path="jquery-1.4.4.js" />
/// <reference path="jquery.ui.message.js" />
/// <reference path="json2.js" />
/// <reference path="columnizer.js" />

var $achievements =
{
    steamUserId: null,

    serviceBase: "Achievement/",

    enableLog: false,

    logSelector: null,

    publishDescription: true,

    init: function (userId, logSelector, enableLog, publishDescription)
    {
        this.steamUserId = userId;
        this.logSelector = logSelector;
        this.enableLog = enableLog;
        this.publishDescription = publishDescription;

        $("img.loading").hide();
    },

    loadProfile: function (selector, callback)
    {
        this.callLoad(selector, "Profile", {}, callback);
    },

    validateProfile: function (steamUserId, callback)
    {
        var data;
        if (steamUserId != null)
        {
            data = { steamUserId: steamUserId };
        }
        else
        {
            data = {};
        }

        this.callAjax("ValidateProfile", data, callback);
    },

    loadGames: function (selector, callback)
    {
        this.callLoad(selector, "Games", {}, callback);
    },

    updateAchievements: function (callback, errorCallback)
    {
        this.callAjax("UpdateAchievements", {}, callback, errorCallback);
    },

    loadUnpublishedAchievements: function (selector, callback)
    {
        this.callLoad(selector, "UnpublishedAchievements", { }, callback);
    },

    hideAchievements: function (achievementIds, callback, errorCallback)
    {
        var parameters = { achievementIds: achievementIds };
        this.callAjax("HideAchievements", parameters, callback, errorCallback);
    },

    publishAchievements: function (achievements, callback, errorCallback)
    {
        // display publish dialog

        var images = new Array();
        var description = new String();
        var gameId = new String();

        $.each(achievements, function (i)
        {
            var achievement = achievements[i];
            images.push({
                type: 'image',
                src: achievement.ImageUrl,
                href: achievement.Game.StatsUrl
            });

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

            if ($achievements.publishDescription)
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

            $achievements.log(description);
        });

        var publishParams = {
            method: 'stream.publish',
            attachment: {
                name: this.steamUserId + " earned new achievements",
                description: description,
                href: "http://steamcommunity.com/id/" + this.steamUserId,
                media: images
            }
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

        FB.ui(publishParams, function (response)
        {
            if (response && response.post_id)
            {
                // on successful publish, update published field on each published achievement.

                var achievementIds = new Array();
                for (var i = 0; i < achievements.length; i++)
                {
                    achievementIds.push(achievements[i].Id);
                }

                var data = { achievementIds: achievementIds };
                $achievements.callAjax("PublishAchievements", data, callback, errorCallback);
            }
        });
    },

    validateSteamUserId: function (errorMessageSelector)
    {
        var valid = true;
        if (this.steamUserId == null || this.steamUserId == "")
        {
            valid = false;
        }

        if (!valid)
        {
            $(errorMessageSelector).message({ type: "error", dismiss: false });
        }

        return valid;
    },

    callLoad: function (selector, method, params, ondone)
    {
        if (params == null)
        {
            params = {};
        }

        // since this is an ajax request, we need to add the signed_request parameter explicitly
        var signedRequest = $("#SignedRequest").val();
        var url = this.serviceBase + method + "?signed_request=" + signedRequest;
        $(selector).load(url, params, ondone);
    },

    callAjax: function (method, query, ondone, onerror)
    {
        if (onerror == null)
        {
            onerror = function (m)
            {
                $achievements.log(m.Message);
            };
        }

        if (query == null)
        {
            query = {};
        }

        // since this is an ajax request, we need to add the signed_request parameter explicitly
        var signedRequest = $("#SignedRequest").val();

        $.ajax({
            url: this.serviceBase + method + "?signed_request=" + signedRequest,
            data: JSON.stringify(query),
            type: "POST",
            processData: true,
            contentType: "application/json",
            timeout: 120000, // 2 minutes
            dataType: "json",
            success: ondone,
            error: function (xhr)
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
                    catch (e)
                    {
                        onerror({ Message: "Unknown server error." });
                    }
                }
                return;
            }
        });
    },

    showLoading: function (selector)
    {
        $(selector).show("normal", this.updateSize);
    },

    hideLoading: function (selector)
    {
        $(selector).fadeOut("slow", this.updateSize);
    },

    updateSize: function ()
    {
        if (FB)
        {
            // update the size of the iframe to match the content
            FB.Canvas.setSize();
        }
    },

    log: function (message)
    {
        if (this.enableLog && this.logSelector != null)
        {
            $(this.logSelector).append(message);
            this.updateSize();
        }
    }
}