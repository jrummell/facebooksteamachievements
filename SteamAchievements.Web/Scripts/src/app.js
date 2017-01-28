if (!window.console) {
    window.console = {};
    if (!window.console.log) {
        window.console.log = function () {
        };
    }
}

(function ($) {
    // MonkeyPatch for Jquery hide() and show() to work with Bootstrap 3
    // https://gist.github.com/zimkies/8360181
    // Bootstrap 3 defines hidden and hide with the !important marker which
    // prevents .show() and .hide() from working on elements that have been
    // hidden using these classes.
    // This patch modifies the hide and show to simply add and remove these
    var hide, show;
    show = $.fn.show;
    $.fn.show = function () {
        this.removeClass("hidden hide");
        return show.apply(this, arguments);
    };
    hide = $.fn.hide;
    return $.fn.hide = function () {
        this.addClass("hidden hide");
        return hide.apply(this, arguments);
    };
})(jQuery);


// button, validation init
$(document).ready(function () {
    $("form .field-validation-error").message({ type: "error", dismiss: false });

    var $toc = $("#toc");
    if ($toc.length > 0) {
        $toc.toc();
    }
});