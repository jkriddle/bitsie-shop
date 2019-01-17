//----------------------------------------
// Log
//----------------------------------------
var Log = Observable.extend({

    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_LIST: 'list',
    
    init: function(){
        this._super();
    },

    "export": function (opt) {
        var self = this;
        window.location = '/Api/Log/Get?export=true&' + $.param(opt);
    },

    // Retrieve log
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Log/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve log list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Log/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    }

});
    