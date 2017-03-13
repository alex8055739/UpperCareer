
$(document).ready(function () {
    $(document).on('mouseenter', '.side-bar > ul > li a', function (e) {
        var currentItem = $(this).closest('li');

        currentItem.siblings('li').find('.content').hide();
        currentItem.find('.content').slideDown();
    });

    $(document).on('mouseleave', '.side-bar > ul > li', function (e) {
        $(this).find('.content').slideUp();
    });

    var $window = $(window),
    stickyDiv = $('.side-bar:last'),
    eTop = stickyDiv.offset().top;
    //eLeft = stickyDiv.offset().left;

    $(window).scroll(function () {
        if ($window.scrollTop() >= eTop - 60) {
            stickyDiv.css('position', 'fixed');
            stickyDiv.css('top', '60px');
            //stickyDiv.css('left', eLeft);
            //stickyDiv.css('width', eWidth);
        }
        else if ($window.scrollTop() < eTop - 60) {
            stickyDiv.removeAttr('style');
        }
    });
});