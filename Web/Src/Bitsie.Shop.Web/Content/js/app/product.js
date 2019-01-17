//----------------------------------------
// Product
//----------------------------------------
var Product = Observable.extend({

    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_DELETED: 'deleted',
    MESSAGE_LIST: 'list',
    MESSAGE_UPDATED: 'updated',
    
    init: function(){
        this._super();
    },

    // Public
    
    // Delete product
    "delete" : function(id) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Product/Delete/' + id, null, function(resp) {
            self.notify(self.MESSAGE_DELETED, id);
        });
    },

    "export": function (opt) {
        var self = this;
        window.location = '/Api/Product/Get?export=true&' + $.param(opt);
    },

    // Retrieve product
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Product/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve product list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Product/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    },

    // Retrieve product
    getSecure: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Product/GetSecure/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Update product data
    create: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Product/Create', data, function (resp) {
            self.notify(self.MESSAGE_CREATED, resp);
        });
    },

    // Update product data
    update: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Product/Update', data, function (resp) {
            self.notify(self.MESSAGE_UPDATED, resp);
        });
    }

});
    