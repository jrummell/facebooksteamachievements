// validation init
$(document).ready(function ()
{
    $("form .field-validation-error").message({ type: "error", dismiss: false });

    $(".footer .about").click(function ()
    {
        $(".footer .disclaimer").toggle();
    });
});