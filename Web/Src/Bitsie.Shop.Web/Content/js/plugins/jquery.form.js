/**
 * Plugin to handle form actions (hiding overlays, disabling elements, etc)
 * Usage:
 * Add Overlay - $('#myForm').form('overlay');
 * Remove Overlay - $('#myForm').form('overlay-hide');
 * Disable Form - $('#myForm').form('disable');
 * Enable Form - $('#myForm').form('enable');
 * Populate form data - $('#myElement').form('populate', formData);
 */
(function ($) {
    $.fn.form = function (action, data) {
        var self = this;
        switch (action) {
            case 'overlay-hide':
                // Remove overlay from DOM
                $('.overlay-container', self).fadeOut(400, function () {
                    $(this).remove();
                });
                break;
            case 'overlay':
                var template = '<div class="overlay-container"><div class="overlay"></div><span><img src="/Content/img/spinner.gif" /> Loading...</span></div>';
                self.prepend(template);
                break;
            case 'enable':
                $('.btn', self).removeAttr("disabled");
                break;
            case 'disable':
                $('.btn', self).attr("disabled", "disabled");
                break;
            case 'populate':
                $.each(data, function (key, value) {
                    var $ctrl = $('[name=' + key + ']', self);
                    switch ($ctrl.attr("type")) {
                        case "text":
                        case "hidden":
                        case "textarea":
                            $ctrl.val(value);
                            break;
                        case "radio":
                        case "checkbox":
                            $ctrl.attr("checked", value);
                            break;
                    }
                });
                break;
        }
    };
})(jQuery);