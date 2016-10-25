/*
 * Upper Shorten plugin for Upper v 1.0.0
 * 
 * Developed by Lai Wei, 2016
 * http://www.uppernews.com
 */

(function ($) {
    $.fn.uppershorten = function (settings) {
        var config = {
            showChar: 140,
            exception: '.self',
            ellipsesText: '...',
            expandBtnText: '查看全部',
            collapseBtnText: '收起',
            controlSection: '.action'
        };

        if (settings) {
            $.extend(config, settings);
        }

        $(document).off('click', '.moretext').off('click', '.lesstext')

        //$(document).on('click', '.moretext', function (e) {
        //    e.preventDefault();
        //    var $this = $(this),
        //        briefTxt = $('#divBrief' + $this.attr('id').replace('aBrief', ''))

        //    if ($this.hasClass('less')) {
        //        $this.removeClass('less').text(config.expandBtnText)
        //    }
        //    else {
        //        $this.addClass('less').text(config.collapseBtnText);
        //    }

        //    //Actions on toggle bref section and original section
        //    briefTxt.toggle();
        //    briefTxt.prev().toggle();
        //})

        $(document).on('click', '.moretext', function (e) {
            e.preventDefault();
            var $this = $(this),
                briefTxt = $('#divBrief' + $this.attr('id').replace('spnMore', ''))

            //Actions on toggle bref section and original section
            briefTxt.toggle();
            briefTxt.prev().toggle();
        })

        $(document).on('click', '.lesstext', function (e) {
            e.preventDefault();
            var $this = $(this),
                briefTxt = $('#divBrief' + $this.attr('id').replace('spnLess', '')),
                offset = briefTxt.prev().offset().top - pageTopOffset;

            //Actions on toggle bref section and original section
            briefTxt.toggle();
            briefTxt.prev().toggle();

            if ($(window).scrollTop() > offset) {
                $('html,body').animate({ scrollTop: offset }, 300);
            }
        })

        return this.not(config.exception).each(function (i) {
            var $this = $(this);
            if ($this.parent().hasClass('shortened')) return;

            $this.parent().addClass('shortened');
            var content = RemoveHtml($this.text());
            var imgCount = $this.find('img').length;

            if (content.length > config.showChar || imgCount > 0) {
                var briefWrap = $(document.createElement('div')).attr('id', 'divBrief' + i).addClass('clearfix').addClass('brief'),
                    text = $(document.createElement('p')).text(content.length > config.showChar ? content.substr(0, config.showChar) + config.ellipsesText : content),
                    btnMore = $(document.createElement('span')).attr('id', 'spnMore' + i).addClass('moretext').html($(document.createElement('a')).attr('href', '#').text(config.expandBtnText)),
                    btnLess = $(document.createElement('span')).attr('id', 'spnLess' + i).addClass('lesstext').html($(document.createElement('a')).attr('href', '#').text(config.collapseBtnText));

                if (imgCount > 0) {
                    var imgWrap = $(document.createElement('div')).addClass('img-wrap'),
                    img = $(document.createElement('img')),
                    firstImg = $this.find('img').first(),
                    imgRatio = firstImg.width() / firstImg.height();

                    img.attr('src', firstImg.attr('src'));

                    if (imgRatio < 1) {
                        imgWrap.width(134);
                        imgWrap.height(134);
                    }
                    else {
                        imgWrap.width(134);
                        imgWrap.height(134 / imgRatio)
                    }

                    imgWrap.html(img);
                    text.prepend(imgWrap);
                }

                briefWrap.append(text);
                $this.after(briefWrap);
                $this.append(btnLess);
                text.append(btnMore);
                $this.hide();
            }
        })
    }
})(jQuery);