/**
 * Mobile
**/
$(document).ready(function () {

    // === Sidebar navigation === //
    $('.submenu > a').click(function (e) {
        e.preventDefault();
        var submenu = $(this).siblings('ul');
        var li = $(this).parents('li');
        var submenus = $('#sidebar li.submenu ul');
        var submenus_parents = $('#sidebar li.submenu');
        if (li.hasClass('open')) {
            if (($(window).width() > 768) || ($(window).width() < 479)) {
                submenu.slideUp();
            } else {
                submenu.fadeOut(250);
            }
            li.removeClass('open');
        } else {
            if (($(window).width() > 768) || ($(window).width() < 479)) {
                submenus.slideUp();
                submenu.slideDown();
            } else {
                submenus.fadeOut(250);
                submenu.fadeIn(250);
            }
            submenus_parents.removeClass('open');
            li.addClass('open');
        }
    });

    function toggleMenu(el, open) {
        if (open) {
            //show up arrow
            $(el).children("i.submenu-carat").removeClass('fa-caret-down');
            $(el).children("i.submenu-carat").addClass('fa-caret-up');
        } else {
            //show down arrow
            $(el).children("i.submenu-carat").removeClass('fa-caret-up');
            $(el).children("i.submenu-carat").addClass('fa-caret-down');
        }
    }

    var ul = $('#sidebar > ul');
    $('#admin-sidebar a').click(function (e) {
        var sidebar = $(this).parent();
        if (sidebar.hasClass('open')) {
           toggleMenu(this, true)
        } else if (!sidebar.hasClass('open')) {
            toggleMenu(this, false);
        }
    });

    // Init sidebar for current page
    $('#admin-sidebar .submenu ul').hide();
    $('#admin-sidebar .submenu').removeClass('open');
    $('#admin-sidebar .active').addClass('open');
    $('#admin-sidebar .active ul').show();
    toggleMenu('#admin-sidebar .active a', true);


    $('#sidebar > a').click(function (e) {
        e.preventDefault();
        var sidebar = $('#sidebar');
        if (sidebar.hasClass('open')) {
            sidebar.removeClass('open');         
            ul.slideUp(250);
        } else {
            sidebar.addClass('open');
            ul.slideDown(250);
        }
    });

    // === Resize window related === //
    $(window).resize(function () {
        if ($(window).width() > 479) {
            ul.css({ 'display': 'block' });
            $('#content-header .btn-group').css({ width: 'auto' });
        }
        if ($(window).width() < 479) {
            ul.css({ 'display': 'none' });
            fix_position();
        }
        if ($(window).width() > 768) {
            $('#user-nav > ul').css({ width: 'auto', margin: '0' });
            $('#content-header .btn-group').css({ width: 'auto' });
        }
    });

    if ($(window).width() < 468) {
        ul.css({ 'display': 'none' });
        fix_position();
    }
    if ($(window).width() > 479) {
        $('#content-header .btn-group').css({ width: 'auto' });
        ul.css({ 'display': 'block' });
    }

    // === Tooltips === //
    $('.tip').tooltip();
    $('.tip-left').tooltip({ placement: 'left' });
    $('.tip-right').tooltip({ placement: 'right' });
    $('.tip-top').tooltip({ placement: 'top' });
    $('.tip-bottom').tooltip({ placement: 'bottom' });

    // === Search input typeahead === //
    $('#search input[type=text]').typeahead({
        source: ['Dashboard', 'Form elements', 'Common Elements', 'Validation', 'Wizard', 'Buttons', 'Icons', 'Interface elements', 'Support', 'Calendar', 'Gallery', 'Reports', 'Charts', 'Graphs', 'Widgets'],
        items: 4
    });
    
    //invoice
    $("#create-invoice").click(function (e) {
        e.preventDefault();
         
    });

    // === Fixes the position of buttons group in content header and top user navigation === //
    function fix_position() {
        var uwidth = $('#user-nav > ul').width();
        $('#user-nav > ul').css({ width: uwidth, 'margin-left': '-' + uwidth / 2 + 'px' });

        var cwidth = $('#content-header .btn-group').width();
        $('#content-header .btn-group').css({ width: cwidth, 'margin-left': '-' + uwidth / 2 + 'px' });
    }

    // === Urgent alerts === //
    if (!$.cookie('hideAlerts')) {
        Bitsie.Shop.Api.get('/api/alert/get', null, function (resp) {
            if (resp.Alerts.length > 0) {
                var html = Bitsie.Shop.Template('#alertsTemplate', resp);
                bootbox.alert(html);
                $.cookie('hideAlerts', true);
            }
        });
    }
});
