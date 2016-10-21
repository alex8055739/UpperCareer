/*
 * Upper scroll paging plugin for Upper v 1.0.0
 * 
 * Developed by Lai Wei, 2016
 * http://www.uppernews.com
 */

(function ($) {
    $.fn.upperscrollpaging = function (url, settings) {
        var config = {
            itemSelector: '.list-group-item',
            contentType: 1,
            postAction: function () { }
        }

        if (settings) {
            $.extend(config, settings);
        }

        var $this = $(this),
            pageIndex = 1,
            onLoading = false,
            spinner = $(document.createElement('img')).attr('src', '/Images/spin.gif').css('margin', '0 auto').css('display', 'block').hide();

        if ($this.length > 1) {
            alert('Cannot bind scroll paging to multiple targets!');
            return false;
        }

        $this.after(spinner)

        $(window).scroll(function () {
            if ($(window).scrollTop() + $(window).height() >= $this.height() + $this.offset().top && !onLoading) {
                var data = new Object();
                data.pageIndex = pageIndex;
                data.id = config.contentType;

                $.ajax(url, {
                    method: 'POST',
                    data: JSON.stringify(data),
                    contentType: 'application/json',
                    dataType: 'html',
                    beforeSend: function () {
                        spinner.fadeIn(1000);
                        onLoading = true;
                    },
                    success: function (result) {
                        var resultCount = $(result).find(config.itemSelector).length;

                        if (resultCount > 0) {
                            pageIndex++;
                            DisplaySuccessInfo('更新了<strong>' + resultCount + '</strong>条新内容，当前页面：<strong>' + pageIndex + '</strong>');
                            $this.find(config.itemSelector).parent().append($(result).find(config.itemSelector));
                            onLoading = false;
                            config.postAction();
                        }
                        else {
                            DisplayErrorInfo('没有更多新内容了，当前页面：<strong>' + pageIndex + '</strong>');
                        }
                    },
                    error: function () {
                        DisplayErrorInfo('对不起，读取新内容时出错，请稍后再试');
                        onLoading = false;
                    },
                    complete: function () {
                        spinner.stop(true, true).hide();
                    }
                })
            }
        });
    }
})(jQuery);