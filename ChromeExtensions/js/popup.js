var settings = {
  ".mkv": true,
  ".mp4": true,
  ".ts": true,
  ".mp4": true,
}
document.addEventListener('DOMContentLoaded', function () {
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
      document.getElementById("message").innerText = "Page not support";
      $("#results").remove();
      $("#copy").remove();
      $("#error").show();
      return;
    }

    chrome.tabs.executeScript(tab.id, {
      code: "document.body.innerHTML"
    }, function (a) {
      var html = a[0];
      var jqueryInstance = $(html);

      chrome
        .storage
        .sync
        .get(['extensions', "customExtensions"], function (results) {
          var extensions = results.extensions || [];
          var customExts = (results.customExtensions || "").split(',');
          for (var x of extensions) {
            settings[x.ext] = x.enabled
          }
          for (var x of customExts) {
            if (x.indexOf(".") < 0) x = "." + x;
            settings[x] = true
          }
          console.log("applied setting", settings)

          var allLinks = jqueryInstance.find('a')
          var urls = "";
          var linkCount = 0;
          allLinks.each((index, el) => {
            var thisUrl = $(el).attr("href");
            var ext = "." + thisUrl.split('.').pop();
            if (thisUrl.indexOf("../") >= 0 || !settings[ext]) return;

            urls += tab.url + $(el).attr("href") + "\n";
            linkCount++;
          });
          document.getElementById("message").innerText = "Found: " + allLinks.length + " links, " + linkCount + " media links";
          document.getElementById("results").innerText = urls;
        });
    });

  });
}, false);