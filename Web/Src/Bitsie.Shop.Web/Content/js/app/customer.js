//----------------------------------------
// User
//----------------------------------------
var Customer = Observable.extend({

    CUSTOMER_RETRIEVED: 'customer-retrieved',
    CUSTOMER_LIST: 'customer-list',
    CUSTOMER_CREATED: 'customer-created',
    CUSTOMER_UPDATED: 'customer-updated',    
    
    init: function(){
        this._super();
    },

    // Public
    create: function (param) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Customer/Create', param, function(resp) {
            self.notify(self.CUSTOMER_CREATED, resp);
        });
    },

    // Update customer data
    update: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Customer/Update', data, function (resp) {
            self.notify(self.CUSTOMER_UPDATED, resp);
        });
    },

    // Retrieve order
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Customer/GetOne/" + id, null, function (resp) {
            self.notify(self.CUSTOMER_RETRIEVED, resp);
        });
    },

    // Retrieve order list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Customer/Get", opt, function (resp) {
            self.notify(self.CUSTOMER_LIST, resp);
        });
    },

});
    