## Configure your Steam Community Profile
You need to first configure your Steam Community profile Custom URL at https://steamcommunity.com/. You must also set your Profile Status to Public.  There is a [Steam Community KB](https://support.steampowered.com/kb_article.php?ref=8882-BMXL-0801) article with more information.

Make a note of your Custom URL suffix:

![](http://www.fbsteamachievements.com/fbsa/canvas/content/images/profilecustomurl.png)

Note: If you change your Steam Community Custom URL, you'll need to also update it in the Steam Achievements application.

## Configure Steam Achievements
On the Settings page, complete your Custom URL and hit Save.

![](http://www.fbsteamachievements.com/fbsa/canvas/content/images/appcustomurl.png)

To have your achievements published automatically, check the Auto Update box and hit Save. This will publish any new achievements you have earned a few times a day.

## Publish Your Achievements
Publish your achievements on the Publish page. Select up to five achievements and click Publish Selected. The post on your wall will look like this:

![](http://www.fbsteamachievements.com/fbsa/canvas/content/images/achievements.png)

Note that only achievements earned within the past two weeks will be displayed on the publish page. You may also see older achievements from games that you've played recently if the unlocked date wasn't recorded.

## View Your Achievements
Click the View Achievements link under any game in the Games box to view your achievements.

## Supported Games
All Steam games that have Steam achievements are supported. For a full list, please visit http://steamcommunity.com/stats/. Please note that this list does not include Games for Windows Live titles purchased from Steam as these games use the Xbox Live network.

## Frequently Asked Questions

### I don't own game X. Why is it my games list?
Some free games, such as Alien Swarm, will show up in your list even if you have never played them. Feel free to ignore them.

### The app doesn't show some of my achievements for game X. Where are they?
Not all achievements are made available to the [Steam Community XML API](https://partner.steamgames.com/documentation/community_data), the API used by Facebook Steam Achievements. It seems that these are maintained separately from the game itself. The games below are known to have missing achievements. We'll update this list if we learn of any others.

  - Alien Swarm
  - Half-Life: Episode 1
  - Half-Life: Episode 2
  - Team Fortress 2
  - Dawn of War II - Retribution

Unfortunately, there's not much we can do about missing achievements. You can help by posting on the [Steam forums](http://forums.steampowered.com/forums/forumdisplay.php?f=316).

### I turned on Auto Update. Why did it publish old achievements?
**Unfortunately, due to the undependable nature of the Steam Community XML API, automatic publishing has been disabled.**

A lot of you have been asking, so here is how Automatic Publishing works. The app looks for new achievements for games that you have played recently (last two weeks). If it found new achievements, it publishes up to the five latest. However, the Steam Community XML API doesn't include the unlock date for some achievements. In this case, the app uses the date that it discovered the achievement.

So if you earned a bunch of achievements in a game that you haven't played for a few months, and then you play it today, Auto Update will then discover your "old" achievements for the first time. If they don't have unlock dates they will get today's date and will be published.

This is a major annoyance of the Steam Community XML API, and I wish there was a better way to work around it. If you're a developer and have a better idea, please let us know!

### Why does nothing happen when I select a few achievements on the Publish page and click publish?
If you have a long list of unpublished achievements, you may have to scroll down the page so see the publish dialog.

### Why do I keep getting the message "Your achievements could not be updated. If this problem continues, please verify that your Custom URL is correct and that your Steam Community Profile is public." when my settings are correct?
Please [verify your settings](#toc-configure-steam-achievements) and try again. If you only see this occasionally, its likely because you are trying to update your achievements when the Steam Community is unavailable.

### How do I remove all information the app collected about my steam profile and Facebook account?
Visit the following link to remove all of your information from the app: https://apps.facebook.com/steamachievementsx/Deauthorize

### Where can I offer suggestions?
If you have an idea on how to improve Steam Achievements for Facebook, please suggest it on the [feedback forum](http://fbsteamachievements.uservoice.com).

### Additional Questions
If you have a question that is not answered here, you can leave a comment on the [feedback forum](http://fbsteamachievements.uservoice.com). You can also post on the [application profile page](http://www.facebook.com/SteamAchievements), but we don't get notified when you do, so you may not get a response.