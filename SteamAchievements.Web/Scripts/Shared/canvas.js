// facebook init
window.fbAsyncInit = function ()
{
    FB.init({
        appId: $("#FacebookAppId").val(),
        channelUrl: $("#ChannelUrl").val(),
        cookie: true,
        status: true,
        xfbml: false
    });
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

// button, validation init
$(document).ready(function ()
{
    $("a.help, button.help, input.help").button({ icons: { primary: "ui-icon-help"} });
    $("a.check").button({ icons: { primary: "ui-icon-check"} });
    $("a.delete").button({ icons: { primary: "ui-icon-trash"} });
    $("a.button").button();

    $("form .field-validation-error").message({ type: "error", dismiss: false });
});