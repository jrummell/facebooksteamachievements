# Steam Achievements for Facebook

Steam Achievements _was_ a web app that displays achievements for your Steam games and allows you to publish them on facebook. It's  built with ASP.NET Core, ASP.NET Identity, facebook login, and Vue.js. You can see it action at <https://fbsteamachievements.azurewebsites.net/>.

[![Build Status](https://dev.azure.com/jrummell/SteamAchievements/_apis/build/status/jrummell.facebooksteamachievements?branchName=main)](https://dev.azure.com/jrummell/SteamAchievements/_build/latest?definitionId=2&branchName=main) ![Deployment status](https://vsrm.dev.azure.com/jrummell/_apis/public/Release/badge/d90c4a42-d007-4648-ad1f-383de65d25eb/2/3)

## How it works

Achievements are parsed from your Steam community profile in XML format:

    http://steamcommunity.com/id/[steamid]/stats/[game]/?xml=1

For example, my Left 4 Dead stats page is <http://steamcommunity.com/id/NullReference/stats/L4D/?xml=1>.

## Help

See the [Help](https://github.com/jrummell/facebooksteamachievements/wiki/help) page.

## Issues and feature requests

Find an issue? Have a great idea for new feature? Please see the [issue tracker](https://github.com/jrummell/facebooksteamachievements/issues).

## Contribute

We're looking for a few developers interested in [contributing](https://github.com/jrummell/facebooksteamachievements/wiki/Contribute) to our project.

## Donate

Do you enjoy using Facebook Steam Achievements? Has this project helped you develop your own Facebook app? Please consider donating to help further development and hosting.

[Donate](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=PRUM27ABHBHXU)
