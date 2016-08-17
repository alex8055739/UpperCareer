function ResizeImage(img) {
    if (img.width() > img.parent().width()) {
        var parentWidth = img.parent().width();
        var imgRatio = img.height() / img.width();
        var adjustedImgHight = imgRatio * parentWidth;

        img.height(adjustedImgHight);
        img.width(parentWidth);

        var newLink = $("<a href='" + img.attr('src') + "' target='_blank' data-toggle='tooltip' data-placement='bottom' title='点击查看大图'>" + img.prop('outerHTML') + "</a>");
        img.parent().prepend(newLink);
        img.remove();
        TriggerTooltip();
    }
}

function Resize() {
    $('img').not('img[class^="portrait"]').each(function () {
        ResizeImage($(this));
    });
}

$(document).ready(function () {
    Resize();
});

$(window).resize(function () {
    Resize();
});