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

    // listen for and handle auth.authResponseChange events
    FB.Event.subscribe('auth.authResponseChange', function (response)
    {
        if (response.status === 'connected')
        {
            // the user is logged in and has authenticated your
            // app, and response.authResponse supplies
            // the user's ID, a valid access token, a signed
            // request, and the time the access token 
            // and signed request each expire
            var uid = response.authResponse.userID;
            var accessToken = response.authResponse.accessToken;

            // update access token and reload
            $.post($("#LogOnUrl").val(), { facebookUserId: uid, accessToken: accessToken },
                function (result)
                {
                    if (result.isValid)
                    {
                        $("#logOnError").hide();
                        window.location = result.redirectUrl;
                    }
                    else
                    {
                        $("#logOnError").text(result.message).message({ type: "error" });
                    }
                });

        } else if (response.status === 'not_authorized')
        {
            // the user is logged in to Facebook, 
            // but has not authenticated your app
        } else
        {
            // the user isn't logged in to Facebook.
        }
    });
};

// Load the SDK Asynchronously
(function (d)
{
    var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement('script'); js.id = id; js.async = true;
    js.src = "//connect.facebook.net/en_US/all.js";
    ref.parentNode.insertBefore(js, ref);
} (document));

// validation init
$(document).ready(function ()
{
    $("form .field-validation-error").message({ type: "error", dismiss: false });

    $(".footer .about").click(function ()
    {
        $(".footer .disclaimer").toggle();
    });
});