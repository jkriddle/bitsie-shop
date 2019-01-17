
(function () {

    if (!window.buyButtonLoaded) {
        window.buyButtonLoaded = true;

        window.onload = function () {
            setButtons();
        }

        setButtons = function () {
            var btns = document.getElementsByClassName('bitsie-buy-button');
            for (var b = 0; b < btns.length; b++) {
                var btn = btns[b];
                btn.innerHTML = '<img src="https://staging-shop.bitsie.com/Content/img/buy-button.jpg" />';
                btn.addEventListener("click", function (e) {
                    e.preventDefault();
                    var merchantId = this.getAttribute('data-merchantid');
                    var productId = this.getAttribute('data-productid');
                    makeFrame(merchantId, productId);
                });
            }
        }


        function makeFrame(merchantId, productId) {
            
            var protocol = window.location.host;
            //for testing local html files
            if (window.location.host == '') {
                var protocol = 'localhost:55354';
            }
            var url = 'https://shop.bitsie.com/' + merchantId + '/Product/Buy/' + productId + "#checkout";
            dialogContainer = document.createElement("DIV");
            dialogContainer.setAttribute("style", "overflow: none; width: 100%; height: 100%; top: 0px; left: 0; position: fixed; z-index: 9999; background: rgba(0,0,0, .90); ");
            dialog = document.createElement("DIV")
            dialog.setAttribute("style", "top:5%; position: relative;  left: 50%; width: 598px; height: 655px; margin-left: -236px; ");
            ifrm = document.createElement("IFRAME");
            ifrm.setAttribute("src", url);
            ifrm.scrolling = "no";
            ifrm.name = "bitsie-buy-screen";
            ifrm.setAttribute("style", "overflow-y:scroll; padding: 20px; position: absolute; top: -25px; width: 100%; height: 100%; border:none");
            dialog.appendChild(ifrm);
            dialogContainer.appendChild(dialog);
            window.parent.document.body.appendChild(dialogContainer);
            dialogContainer.addEventListener("click", function (e) {
                this.remove();
            });
        }
    }
})();