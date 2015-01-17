## How automatic publishing works

**Unfortunately, due to the undependable nature of the Steam Community XML API, automatic publishing has been disabled.**

A lot of you have been asking, so here is how Automatic Publishing works. The app looks for new achievements for games that you have played recently (last two weeks). If it found new achievements, it publishes up to the five latest. However, the Steam Community XML API doesn't include the unlock date for some achievements. In this case, the app uses the date that it discovered the achievement.

So if you earned a bunch of achievements in a game that you haven't played for a few months, and then you play it today, Auto Update will then discover your "old" achievements for the first time. If they don't have unlock dates they will get today's date and will be published.

This is a major annoyance of the Steam Community XML API, and I wish there was a better way to work around it. If you're a developer and have a better idea, please let us know!