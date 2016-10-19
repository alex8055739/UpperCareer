var pageIndex = 0;

function ResetPageIndex() {
    pageIndex = 0;
}

$(document).ready(function () {
    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() == $(document).height()) {
            pageIndex++;
            alert(pageIndex);
        }
    });
});
