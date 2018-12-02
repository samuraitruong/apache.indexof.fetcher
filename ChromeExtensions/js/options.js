function saveOptions() {
    //window.close();
    const exts = [];
    $("input").each(function (index, el) {
        const data = $(this).data();
        exts.push({
            ext: data.ext,
            enabled: $(this).prop('checked')
        })
    });
    console.log(exts)
    chrome
        .storage
        .sync
        .set({
            "extensions": exts,
            "customExtensions": $("#txtCustomExt").val()
        }, function () {
            console.log("aaa")
            window.close();
        });

}

function loadOptions() {
    chrome
        .storage
        .sync
        .get(['extensions', 'customExtensions'], function (results) {
            console.log(results.extensions)
            $("input").each(function (index, el) {
                const data = $(this).data();
                const filter = results
                    .extensions
                    .find(function (x) {
                        return x.ext == data.ext
                    });
                console.log('filter', filter);
                $(this).prop('checked', filter.enabled);
                $("#txtCustomExt").val(results.customExtensions || "")
            });

        });
}
$(document)
    .ready(function () {
        $("#close").click((function () {
            window.close();
        }));

        $("#save").click(saveOptions);
        loadOptions();
    });