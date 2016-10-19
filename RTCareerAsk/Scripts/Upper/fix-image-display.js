function ResizeImage(img, containerClass) {
    var parent = img.parent(),
        container = img.closest('.' + containerClass);

    if (container.is(':hidden')) {
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
    $('img').not('img[class^="portrait"]').each(function () {
        ResizeImage($(this), containerClass);
    });
}

$(document).ready(function () {
    Resize('content');
});

$(window).resize(function () {
    Resize('content');
});