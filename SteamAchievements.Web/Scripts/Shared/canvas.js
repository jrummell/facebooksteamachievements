// button, validation init
$(document).ready(function ()
{
    $("a.help, button.help, input.help").button({ icons: { primary: "ui-icon-help"} });
    $("a.check").button({ icons: { primary: "ui-icon-check"} });
    $("a.delete").button({ icons: { primary: "ui-icon-trash"} });
    $("a.button").button();

    $("form .field-validation-error").message({ type: "error", dismiss: false });
});