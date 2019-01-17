//----------------------------------------
// Report
//----------------------------------------
var Report = Observable.extend({

    REPORT_LIST: 'report-list',
    
    init: function(){
        this._super();
    },

    // Payout report
    payout: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Report/Payout", opt, function (resp) {
            self.notify(self.REPORT_LIST, resp);
        });
    }

});
    