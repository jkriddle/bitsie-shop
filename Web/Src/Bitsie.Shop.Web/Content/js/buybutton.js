window.onload = function () {
    var btns = document.getElementsByClassName("bitsie-shop-btn");
    function buttonClick(e) {
        var btn = e.toElement;
        var title = btn.getAttribute('data-title');
        var price = btn.getAttribute('data-price');
        var description = btn.getAttribute('data-description');
    };
    for (var i = 0; i < btns.length; i++) {
        btns[i].addEventListener('click', buttonClick, true);
    };
}


