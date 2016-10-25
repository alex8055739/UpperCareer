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
            pageInfo = "未知内容",
            loader = $(document.createElement('div')).addClass('preload-box').html($(document.createElement('div')).addClass('preload-3')).hide();

        switch (config.contentType) {
            case 1:
                pageInfo = "最热问题";
                break;
            case 2:
                pageInfo = "最新问题";
                break;
            case 3:
                pageInfo = "最热答案";
                break;
            case 4:
                pageInfo = "最新答案";
                break;
            default:
                break;
        }

        if ($this.length > 1) {
            alert('Cannot bind scroll paging to multiple targets!');
            return false;
        }

        $this.after(loader)

        $(window).off('scroll');

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
                        loader.fadeIn(1000);
                        onLoading = true;
                    },
                    success: function (result) {
                        var resultCount = $(result).find(config.itemSelector).length;

                        if (resultCount > 0) {
                            pageIndex++;
                            DisplaySuccessInfo('<strong>' + pageInfo + '</strong> 更新了<strong>' + resultCount + '</strong>条新内容，当前页面：<strong>' + pageIndex + '</strong>');
                            $this.find(config.itemSelector).parent().append($(result).find(config.itemSelector));
                            onLoading = false;
                            config.postAction();
                        }
                        else {
                            DisplayErrorInfo('<strong>' + pageInfo + '</strong> 没有更多新内容了，当前页面：<strong>' + pageIndex + '</strong>');
                        }
                    },
                    error: function () {
                        DisplayErrorInfo('对不起，读取新内容时出错，请稍后再试');
                        onLoading = false;
                    },
                    complete: function () {
                        loader.stop(true, true).hide();
                    }
                })
            }
        });
    }
})(jQuery);