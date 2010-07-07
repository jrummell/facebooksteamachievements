<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="false"
    CodeBehind="PublishDialogTest.aspx.cs" Inherits="SteamAchievements.PublishDialogTest" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
    <input type="button" value="Publish" onclick="publishTest();" />
    <script src="http://connect.facebook.net/en_US/all.js" type="text/javascript"></script>
    <script type="text/javascript">
        function publishTest()
        {
            var images = [
            {
                type: 'image',
                src: 'http://media.steampowered.com/steamcommunity/public/images/apps/240/7bbd09bac4ebe17b84e8fb0eb5d9e3351fcb4bc0.jpg',
                href: 'http://steamcommunity.com/id/nullreference/stats/CS:S?tab=achievements'
            },
            {
                type: 'image',
                src: 'http://media.steampowered.com/steamcommunity/public/images/apps/240/a646cdeba4eb3590fb1b16241949daec306df333.jpg',
                href: 'http://steamcommunity.com/id/nullreference/stats/CS:S?tab=achievements'
            }];

            var publishParams = {
                method: 'stream.publish',
                message: 'Steam Achievements Test',
                attachment: {
                    name: 'Test',
                    caption: 'A test caption',
                    description: 'Testing steam.publish from Steam Achievements Test',
                    href: 'http://apps.facebook.com/steamachievementsxt/'
                },
                media: images
            };

            FB.ui(publishParams, publishCallback);
        }

        function publishCallback(response)
        {
            if (response && response.post_id)
            {
                alert('Post was published.');
            } else
            {
                alert('Post was not published.');
            }
        }
    </script>
</asp:Content>
