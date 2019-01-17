(function(){

	window.onload = function(){
		var btns = document.getElementsByClassName('bitsie-buy-button');
		for(var b = 0; b < btns.length; b++){
			var btn = btns[b];
			btn.addEventListener("click", function(e){
				var title = this.getAttribute('data-title');
				var description = this.getAttribute('data-description')
				var price = this.getAttribute('data-price');
				makeFrame();
			});
		}	
	}

	function makeFrame() { 
		dialogContainer = document.createElement("DIV");
		dialogContainer.setAttribute("style", "width: 100%; height: 100%; top: 0; left: 0; position: fixed; z-index: 9999; background: black; opacity: .75");
		dialog = document.createElement("DIV")
		dialog.setAttribute("style", "top:10%; position: fixed; left: 50%; width: 598px; height: 472px; margin-left: -236px; -webkit-box-shadow: 0 0 10px 0 #595959; box-shadow: 0 0 10px 0 #595959;");
		ifrm = document.createElement("IFRAME"); 
		ifrm.setAttribute("src", "https://shop.bitsie.com/rjN3Hr/checkout");
		ifrm.style.width = "100%"; 
		ifrm.style.border = "none";
		ifrm.style.height = "100%"; 
		dialog.appendChild(ifrm);
		dialogContainer.appendChild(dialog);
		document.body.appendChild(dialogContainer); 
		dialogContainer.addEventListener("click", function(e){
			this.remove();
		});
	}	 
	
})();