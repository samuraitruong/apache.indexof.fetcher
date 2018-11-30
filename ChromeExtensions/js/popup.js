document.addEventListener('DOMContentLoaded', function() {
  ///var btnExpandFrame = document.getElementById('expandFrame');
  //btnExpandFrame.addEventListener('click', function() {
    var close = document.getElementById('close');
    close.addEventListener('click', function() {
        window.close();
    });
    chrome.tabs.getSelected(null, function(tab) {

      console.log("current tab", tab)

        chrome.tabs.executeScript(tab.id, { code: "document.body.innerHTML"}, function(a) {
            var html = a[0];
            console.log("callback", html); 
            var x = $(html);

            document.getElementById("button1").innerText =x.find('a').length;
        });
      
    });
  //}, false);
}, false);