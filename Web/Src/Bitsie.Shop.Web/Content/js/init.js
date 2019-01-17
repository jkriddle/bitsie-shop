$(function() {

    // Register Handelbars partials
    $('.hb-partial').each(function (i, el) {
        var partialName = $(el).data('name');
        Handlebars.registerPartial(partialName, $(el).html());
    });

    // Allow clicks on inline icons to focus on the proper input
    $('.icon-inline').live('click', function() {
        $(this).siblings('input:first-child').trigger('focus');
    });
    
    // Toggle buttons
    $('.toggle-button').toggleButtons();
    
})