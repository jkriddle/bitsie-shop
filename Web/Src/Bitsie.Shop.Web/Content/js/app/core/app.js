// Console log for unsupported browsers
var debugging = true; // or true
if (typeof console == "undefined") var console = { log: function () { } };
else if (!debugging || typeof console.log == "undefined") console.log = function () { };

// Handlebars helper to determine if two values are equal.
Handlebars.registerHelper('equals', function (p1, p2, options) {
    return p1 == p2 ? options.fn(this) : null;
});

// Handlebars helper to determine if two values are not equal.
Handlebars.registerHelper('notEquals', function (p1, p2, options) {
    return p1 != p2 ? options.fn(this) : null;
});

var Bitsie = {};
Bitsie.Shop = {};

/* Configurations
 *************************************************************/
Bitsie.Shop.Config = {};

Bitsie.Shop.Config.dateRanges = {
    'Today': [moment(), moment()],
    'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)],
    'Last 7 Days': [moment().subtract('days', 6), moment()],
    'Last 30 Days': [moment().subtract('days', 29), moment()],
    'This Month': [moment().startOf('month'), moment().endOf('month')],
    'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
};

/* Querystring
 *************************************************************/
// Create a "QueryString" object containing all URL parameters.
// Usage: Bitsie.Shop.QueryString.myParam (?myParam=foo)
Bitsie.Shop.QueryString = function () {
    // This function is anonymous, is executed immediately and 
    // the return value is assigned to QueryString!
    var queryString = {};
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        // If first entry with this name
        if (typeof queryString[pair[0]] === "undefined") {
            queryString[pair[0]] = pair[1];
            // If second entry with this name
        } else if (typeof queryString[pair[0]] === "string") {
            var arr = [queryString[pair[0]], pair[1]];
            queryString[pair[0]] = arr;
            // If third or later entry with this name
        } else {
            queryString[pair[0]].push(pair[1]);
        }
    }
    return queryString;
}();

/**
 * Scroll to a position on the page
 */
Bitsie.Shop.scrollTo = function (position) {
    $("html, body").animate({ scrollTop: position }, "slow");
}

/* Templating
 *************************************************************/
// Helper to generate Handlebars template from jQuery ID
Bitsie.Shop.Template = function (el, ob) {
    if (!ob) ob = {};
    var html = $(el).html();
    if (!html) return null;
    var temp = Handlebars.compile(html);
    return temp(ob);
};

// Helper to generate handlebars template from raw HTML
Bitsie.Shop.Template.raw = function (html, ob) {
    if (!ob) ob = {};
    var temp = Handlebars.compile(html);
    if (!html) return null;
    return temp(ob);
};

/* API
 *************************************************************/
Bitsie.Shop.Api = {

    Auth: {
        setToken: function (token, expires) {
            var secure = window.location.href.indexOf("shop.bitsie.com") != -1;
            $.cookie('authToken', token, { expires: expires, path: '/', secure: secure });
        },
        getToken: function (token) {
            return $.cookie('authToken');
        },
        clearToken: function () {
            $.removeCookie('authToken', { path: '/' });
        }
    },

    // Send a GET request
    get: function (url, data, success, error) {
        Bitsie.Shop.Api._sendRequest(url, data, 'GET', success, error);
    },

    // Send a POST request
    post: function (url, data, success, error) {
        Bitsie.Shop.Api._sendRequest(url, data, 'POST', success, error);
    },

    // Private function to handle all API traffic
    _sendRequest: function (url, data, method, success, error) {
        if (error == undefined) {
            error = function (xhr, status, p3, p4) {
                if (xhr.status == "401") {
                    window.location = '/user/signin';
                    return;
                }
                var err = "";
                if (xhr.responseText && xhr.responseText[0] == "{") {
                    var obj = JSON.parse(xhr.responseText);
                    // .net error
                    if (obj.ExceptionMessage) err = obj.ExceptionMessage;
                    else if (obj.Message) err = obj.Message;
                    else err = "Oops, an error has occurred. Please try again."
                }
                else {
                    err = "An error has occurred: " + status + " " + p3;
                }

                // User login has expired.
                if (obj && obj.ExceptionMessage == "Unable to validate your device credentials.") {
                    window.location.href = '/User/SignIn?Message=You+have+been+logged+out.';
                    return;
                }

                if (bootbox != undefined) bootbox.alert(err);
                else alert(err);
            };
        }

        var headers = { "AuthToken": Bitsie.Shop.Api.Auth.getToken() };

        // CSRF
        if (method == "POST") {
            var token = $('input[name="__RequestVerificationToken"]').val();
            if (token) {
                headers["__RequestVerificationToken"] = token;
            }
        }

        $.ajax({
            url: url,
            type: method,
            data: data,
            headers : headers,
            success: success,
            headers: headers,
            error: error,
            cache: false
        });
    },
};