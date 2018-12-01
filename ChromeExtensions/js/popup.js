document.addEventListener('DOMContentLoaded', function () {
  ///var btnExpandFrame = document.getElementById('expandFrame');
  //btnExpandFrame.addEventListener('click', function() {
  var close = document.getElementById('close');
  close.addEventListener('click', function () {
    window.close();
  });

  document.getElementById("copy").addEventListener("click", function () {
    var el = document.getElementById("results");
    el.select();
    document.execCommand('copy');
  });

  chrome.tabs.getSelected(null, function (tab) {

    console.log("current tab", tab)
    if (tab.title.indexOf("Index of") < 0) {
      document.getElementById("message").innerText = "Sorry, This extension only function on Apache Index page."
      return;
    }

    chrome.tabs.executeScript(tab.id, {
      code: "document.body.innerHTML"
    }, function (a) {
      var html = a[0];
      // html = html.replace("<pre>", "<div>");
      // html.replace("</pre>", "</div>");
      var x = $(html);

      document.getElementById("message").innerText = "Found: " + x.find('a').length + " links";
      var urls = "";
      x.find('a').each((index, el) => {
        urls += tab.url + $(el).attr("href") + "\n";
      });
      document.getElementById("results").innerText = urls;

    });

  });
  //}, false);
}, false);