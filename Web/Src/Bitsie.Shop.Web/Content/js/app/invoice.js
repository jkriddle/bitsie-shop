//----------------------------------------
// User
//----------------------------------------
var Invoice = Observable.extend({

    INVOICE_RETRIEVED: 'invoice-retrieved',
    INVOICE_LIST: 'invoice-list',
    INVOICE_CREATED: 'invoice-created',
    INVOICE_UPDATED: 'invoice-updated',
    INVOICE_PAYOUT: 'invoice-payout',
    INVOICE_SENT: 'invoice-sent',
    INVOICE_COUNT: 'invoice-count',
    
    init: function(){
        this._super();
    },

    // Public
    create: function (param) {
        var self = this;
        Bitsie.Shop.Api.post('/Api/Invoice/Create', param, function(resp) {
            self.notify(self.INVOICE_CREATED, resp);
        });
    },

    update: function (data) {
        var self = this;
        console.log(data);
        Bitsie.Shop.Api.post('/Api/Invoice/Update' , data, function(resp) {
            self.notify(self.INVOICE_UPDATED, resp);
        });
    },

    // Retrieve order
    getOne: function (id) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Invoice/GetOne/" + id, null, function (resp) {
            self.notify(self.INVOICE_RETRIEVED, resp);
        });
    },

    // Retrieve order list
    get: function (opt) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Invoice/Get", opt, function (resp) {
            self.notify(self.INVOICE_LIST, resp);
        });
    },
    
    getCount: function () {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Invoice/GetInvoiceCount", Option, function(resp) {
            self.notify(self.INVOICE_COUNT, resp);
        });
    },
    
    send: function (invoiceId) {
        var self = this;
        Bitsie.Shop.Api.get("/Api/Invoice/Send/" + invoiceId, null, function (resp) {
            self.notify(self.INVOICE_SENT, resp);
        });
    }

});

function InvoiceList(listContainer, rowTemplate) {

    var _this = this;
    this.getInvoiceItemArray = function () {

          var invoiceArray = new Array();
          var invoiceNum = 0;
          $(".invoice-item:not(#invoice-item-template)").each(function(index, tr) {

              invoiceObject = new Object();
                invoiceObject.Position = index;
                invoiceObject.UsdAmount = $(tr).find('input.amount').val();
                invoiceObject.Quantity = $(tr).find('input.quantity').val();
                invoiceObject.Description = $(tr).find('input.description').val();
                if (invoiceObject.Description != '' && invoiceObject.UsdAmount != '' && invoiceObject.Quantity != null) {
                  invoiceArray[invoiceNum] = invoiceObject;
                }
                
              invoiceNum++;
          });
          return invoiceArray;
    };

    this.render = function() {
        $(listContainer).find('.invoice-item').each(function() {
            _this.calculateInvoice(this);
        });
    };

    this.calculateInvoice = function (row) {

        //calculate total of row
        var quantity = $(row).find('input.quantity').val();
        var amount = $(row).find('input.amount').val();
        var subtotal = quantity * amount;
        //find subtotal td
        $(row).find('.itemTotal').children("input").val(subtotal);
        
        
        //calculate total invoice
        var total = 0;
        $(listContainer).find('.itemTotal').each(function () {
            var currValue = $(this).children("input").val();
            total += +currValue;
        });
        console.log(total);
        $(".invoice-total").html(total);
        $("input#amount").val(total);

    };

    $(listContainer).on("click", ".add-invoice-item", function (e) {
        var addRow = $(rowTemplate).clone().removeClass("hide").attr("id", "");
        $(this).parent().parent().after($(addRow));
        console.log($("invoice-item:not(#invoice-item-template)").length);
        if ($(".invoice-item:not(#invoice-item-template)").length > 1) {
            $(".invoice-item:not(#invoice-item-template)").find('.remove-invoice-item').show();
        } else {
            $(".invoice-item:not(#invoice-item-template)").find('.remove-invoice-item').hide();
        }
    });

    $(listContainer).on("click", ".remove-invoice-item", function (e) {
        $(this).parent().parent().fadeOut('fast', function() {
            $(this).remove();
        });
        console.log($(".invoice-item:not(#invoice-item-template)").length);
        if($(".invoice-item:not(#invoice-item-template)").length == 2) {
            $(".invoice-item:not(#invoice-item-template)").find('.remove-invoice-item').hide();
        }else {
           $(".invoice-item:not('#invoice-item-template')").find('.remove-invoice-item').show();
        }
    });

    $(listContainer).on("keyup", "input.amount", function (e) {
        _this.calculateInvoice($(this).parent().parent());
    });
    
    $(listContainer).on("keyup", "input.quantity", function (e) {
        _this.calculateInvoice($(this).parent().parent());
    });
    
};
    