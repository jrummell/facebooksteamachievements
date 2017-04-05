// facebook init
window.fbAsyncInit = function ()
{
    FB.init({
        appId: $("#FacebookAppId").val(),
        channelUrl: $("#ChannelUrl").val(),
        cookie: true,
        xfbml: true,
        oauth: true,
        status: true
    });

    if ($("#LogOnView").length == 0)
    {
        return;
    }
};

(function (d)
{
    var js, id = 'facebook-jssdk';
    if (d.getElementById(id))
    {
        return;
    }
    js = d.createElement('script');
    js.id = id;
    js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    d.getElementsByTagName('head')[0].appendChild(js);
} (document));