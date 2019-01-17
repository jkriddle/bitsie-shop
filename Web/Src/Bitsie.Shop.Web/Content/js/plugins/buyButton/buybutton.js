
(function () {
    
    if (!window.buyButtonLoaded) {
        console.log("loaded");
        window.buyButtonLoaded = true;

        window.onload = function () {

            setButtons();
        }

        setButtons = function () {


            var btns = document.getElementsByClassName('bitsie-buy-button');
            for (var b = 0; b < btns.length; b++) {
                var btn = btns[b];
                btn.innerHTML = '<img src="http://localhost:55354/Content/img/buy-button-background.jpg" />';
                btn.addEventListener("click", function (e) {
                    e.preventDefault();
                    var merchantId = this.getAttribute('data-merchantId');
                    var productId = this.getAttribute('data-productId');
                    var param = new Object();
                    param.MerchantId = merchantId;
                    param.ProductId = productId;
                    createOrder(param);
                });
            }
        }

        createOrder = function (param) {
            var self = this;
            xmlhttp.open("GET", "ajax_info.txt", false);
            xmlhttp.send();
            document.getElementById("myDiv").innerHTML = xmlhttp.responseText;

            $.ajax({
                url: 'http://localhost:57361/Order/Create?',
                type: 'POST',
                data: param,
                success: function (resp) {
                    makeFrame(resp.Order.OrderNumber, param.MerchantId);
                },
                error: function (err) {
                    console.log("err---");
                    console.log(err);
                },
                cache: false
            });
        }


        function makeFrame(orderNumber, merchantId) {

            if (window.location.host == '') {
                console.log("test");
            }
            var protocol = 'localhost:55354';

            var url = "http://" + protocol + '/' + merchantId + '/checkout?orderNumber=' + orderNumber

            dialogContainer = document.createElement("DIV");
            dialogContainer.setAttribute("style", "overflow-y:scroll; width: 100%; height: 100%; top: 0; left: 0; position: fixed; z-index: 9999; background: rgba(0,0,0, .90); ");
            dialog = document.createElement("DIV")
            dialog.setAttribute("style", "top:5%; position: relative;  left: 50%; width: 598px; height: 635px; margin-left: -236px; ");
            ifrm = document.createElement("IFRAME");
            ifrm.setAttribute("src", url);
            ifrm.scrolling = "yes";
            ifrm.setAttribute("style", "overflow-y:scroll; padding: 20px; position: absolute; top: -50px; width: 100%; height: 100%; border:none");
            dialog.appendChild(ifrm);
            dialogContainer.appendChild(dialog);
            window.parent.document.body.appendChild(dialogContainer);
            dialogContainer.addEventListener("click", function (e) {
                this.remove();
            });
        }
    }
})();