//----------------------------------------
// User
//----------------------------------------
var Order = Observable.extend({

    ORDER_RETRIEVED: 'order-retrieved',
    ORDER_LIST: 'order-list',
    ORDER_CREATED: 'order-created',
    ORDER_UPDATED: 'order-updated',
    ORDER_PAYOUT: 'order-payout',
    
    init: function(){
        this._super();
    },

    // Public
    create: function (param) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Order/Create', param, function (resp) {
            self.notify(self.ORDER_CREATED, resp);
        })
    },

    "export" : function(opt) {
        var self = this;
        window.location = '/Api/Order/Get?export=true&' + $.param(opt);
    },

    update: function (orderNumber) {
        var self = this;
        Bitsie.Shop.Api.get('/Api/Order/Update/' + orderNumber, null, function (resp) {
            self.notify(self.ORDER_UPDATED, resp);
        })
    },

    // Retrieve order
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Order/GetOne/" + id, null, function (resp) {
            self.notify(self.ORDER_RETRIEVED, resp);
        });
    },

    // Retrieve order list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Order/Get", opt, function (resp) {
            self.notify(self.ORDER_LIST, resp);
        });
    },

    // Submit order payouts
    payout: function (opt) {
        var self = this;
        Bitsie.Shop.Api.post("/Api/Order/Payout", opt, function (resp) {
            self.notify(self.ORDER_PAYOUT, resp);
        });
    }


});
    