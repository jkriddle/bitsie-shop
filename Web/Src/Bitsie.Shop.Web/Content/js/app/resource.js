//----------------------------------------
// Resource
//----------------------------------------
var Resource = Observable.extend({

    MESSAGE_RETRIEVED: 'retrieved',
    MESSAGE_DELETED: 'deleted',
    MESSAGE_LIST: 'list',
    MESSAGE_UPDATED: 'updated',
    
    init: function(){
        this._super();
    },

    // Public

    // Retrieve resource
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Resource/GetOne/" + id, null, function (resp) {
            self.notify(self.MESSAGE_RETRIEVED, resp);
        });
    },

    // Retrieve resource list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Resource/Get", opt, function (resp) {
            self.notify(self.MESSAGE_LIST, resp);
        });
    },

    // Update resource data
    update: function (data) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Resource/Update', data, function (resp) {
            self.notify(self.MESSAGE_UPDATED, resp);
        });
    }
});
    