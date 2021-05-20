$("document").ready(function () {
    $(function () {
        $('.cssmenu a[href="' + location.pathname.split("/")[location.pathname.split("/").length - 1] + '"]').parent().addClass('active');
    });

});