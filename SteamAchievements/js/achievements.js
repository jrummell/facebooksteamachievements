/// <reference path="js/jquery-vsdoc.js" />
/// <reference path="js/json2.js" />

var $achievements =
{
    steamUserId: null,

    serviceBase: "Services/Achievement.svc/",

    enableLog: false,

    logSelector: null,

    init: function (userId, logSelector, enableLog)
    {
        this.steamUserId = userId;
        this.logSelector = logSelector;
        this.enableLog = enableLog;

        $("img.loading").hide();

        // hide messages on click
        $("div.message").append(" <span class='dismiss'>Click to dismiss.</span>").click(function ()
        {
            $(this).hide('normal', $achievements.updateSize);
        });
    },

    getGames: function (callback, errorCallback)
    {
        this.callAjax("GetGames", { steamUserId: this.steamUserId }, callback, errorCallback);
    },

    updateAchievements: function (callback, errorCallback)
    {
        var ondone = function (updateCount)
        {
            $achievements.callAjax("GetNewAchievements", { steamUserId: $achievements.steamUserId }, callback, errorCallback);
        };

        var parameters = { "steamUserId": this.steamUserId };
        this.callAjax("UpdateAchievements", parameters, ondone, errorCallback);
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

            description += achievement.Name + " (" + achievement.Description + ")";

            if (i < achievements.length - 1)
            {
                description += ", ";
            }
            else
            {
                description += ".";
            }
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
                for (i = 0; i < achievements.length; i++)
                {
                    achievementIds.push(achievements[i].Id);
                }

                var data = { steamUserId: $achievements.steamUserId, achievementIds: achievementIds };
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
            this.showMessage(errorMessageSelector);
        }

        return valid;
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

        $.ajax({
            url: this.serviceBase + method,
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

    showMessage: function (selector)
    {
        $(selector).show("normal", this.updateSize);
    },

    updateSize: function ()
    {
        // update the size of the iframe to match the content
        FB.Canvas.setSize();
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