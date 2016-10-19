(function ($) {
    $.fn.uppertabs = function () {
        return this.each(function (i) {
            var $activeWidth, $defaultMarginLeft, $defaultPaddingLeft, $firstChild, $line, $navListItem, $activeItem;
            var $this = $(this);
            $line = $this.find('.line');
            $navListItem = $this.find('.tab-li');
            $activeItem = $this.find('.active-nav');
            $activeWidth = $this.find('.active-nav').width();
            $firstChild = $this.find('.tab-li:first-child');
            $defaultMarginLeft = parseInt($this.find('.tab-li:first-child').next().css('margin-left').replace(/\D/g, ''));
            $defaultPaddingLeft = parseInt($this.find('.tab-container > ul').css('padding-left').replace(/\D/g, ''));
            $line.width($activeWidth + 'px');
            $line.css('marginLeft', $activeItem.index() == 0 ? $defaultPaddingLeft + 'px' : $defaultMarginLeft + $activeItem.position().left + 'px');

            return $navListItem.click(function () {
                var $activeNav, $currentIndex, $currentOffset, $currentWidth, $initWidth, $marginLeftToSet, $this;
                $this = $(this);
                $activeNav = $navListItem.filter('.active-nav');
                $currentWidth = $activeNav.width();
                $currentOffset = $activeNav.position().left;
                $currentIndex = $activeNav.index();
                $activeNav.removeClass('active-nav');
                $this.addClass('active-nav');
                if ($this.index() > $currentIndex) {
                    if ($activeNav.is($firstChild)) {
                        $initWidth = $defaultMarginLeft + $this.width() + $this.position().left - $defaultPaddingLeft;
                    } else {
                        $initWidth = $this.position().left + $this.width() - $currentOffset;
                    }
                    $marginLeftToSet = $this.position().left + $defaultMarginLeft + 'px';
                    $line.width($initWidth + 'px');
                    return setTimeout(function () {
                        $line.css('marginLeft', $marginLeftToSet);
                        return $line.width($this.width() + 'px');
                    }, 175);
                } else {
                    if ($this.is($firstChild)) {
                        $initWidth = $currentOffset - $defaultPaddingLeft + $defaultMarginLeft + $currentWidth;
                        $marginLeftToSet = $this.position().left;
                    } else {
                        $initWidth = $currentWidth + $currentOffset - $this.position().left;
                        $marginLeftToSet = $this.position().left + $defaultMarginLeft;
                    }
                    $line.css('marginLeft', $marginLeftToSet);
                    $line.width($initWidth + 'px');
                    return setTimeout(function () {
                        return $line.width($this.width() + 'px');
                    }, 175);
                }
            });
        })
    }
})(jQuery);