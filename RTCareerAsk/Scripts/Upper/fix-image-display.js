function ResizeImage(img) {
    var parent = img.parent();

    if (img.is(':hidden')) {
        return false
    }

    if (img.width() > parent.width() && !parent.is('a')) {
        var parentWidth = parent.width(),
            imgRatio = img.height() / img.width(),
            adjustedImgHight = imgRatio * parentWidth;

        img.height(adjustedImgHight);
        img.width(parentWidth);

        var newLink = $("<a href='" + img.attr('src') + "' target='_blank' data-toggle='tooltip' data-placement='bottom' title='点击查看大图'>" + img.prop('outerHTML') + "</a>");
        img.replaceWith(newLink);
        TriggerTooltip();
    }
}

function Resize(containerClass) {
    $('img').not('img[class^="portrait"]').not('.photo').not('.nav-img').each(function () {
        ResizeImage($(this));
    });
}

$(document).ready(function () {
    Resize();
});

$(window).resize(function () {
    Resize();
});