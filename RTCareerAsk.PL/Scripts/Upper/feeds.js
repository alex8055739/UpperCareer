function AfterAnswerFeedsLoad() {
    Resize();
    $('div[id^="divAnswer"]').uppershorten();
}

function UpdateAnswerCmtCount(ansId, isIncrement) {
    var badges = $('span[id^="spnCmtCnt' + ansId + '"]')

    badges.each(function () {
        var originCount = parseInt($(this).text()),
            change = isIncrement == true ? 1 : -1;

        $(this).text(originCount + change);
    })
}

function DisplayCommentError(infoText) {
    var errorTab = $('#divInfoFeedComment').find('p');
    $('div[id^="divInfo"]').stop(true, true).hide();
    errorTab.html(infoText);
    errorTab.closest('.alert-tag').slideDown('slow').delay(2000).slideUp('slow');
}

$(document).ready(function () {
    AfterAnswerFeedsLoad();

    $(document).on('click', '.question-collapse', function () {
        var $this = $(this),
            classOpen = 'opened';

        if ($this.hasClass(classOpen)) {
            $this.removeClass(classOpen).html('<span class="glyphicon glyphicon-plus"></span>&nbsp;查看详情');
        }
        else {
            $this.addClass(classOpen).html('<span class="glyphicon glyphicon-minus"></span>&nbsp;收起');
        }
    });

    $('.feed-list > ul').upperscrollpaging('/Home/LoadFeedsByPage', {
        postAction: AfterAnswerFeedsLoad
    })

    $(document).on('click', '.feed-comments', function (e) {
        e.preventDefault();

        var $this = $(this),
            modal = $('#divModal'),
            ansId = $this.data('id')

        $.ajax('/Home/LoadFeedAnswerComments', {
            type: "POST",
            data: JSON.stringify({ ansId: ansId }),
            contentType: 'application/json',
            dataType: "html",
            success: function (result) {
                modal.children().html(result);
                modal.modal('show');
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    });

    $(document).on('keyup', '.feed-comment .textbox', function (e) {
        var $this = $(this),
            text = $this.text(),
            placeholder = $this.parent().siblings('.placeholder');

        if (text) {
            placeholder.hide();
        }
        else {
            placeholder.show();
        }
    });

    $(document).on('click', '.feed-comment .placeholder', function (e) {
        var $this = $(this),
            textbox = $this.siblings('.text').children('.textbox');

        textbox.focus();
    });

    $(document).on('click', '.feed-comment .text .btn', function (e) {
        e.preventDefault();

        var $this = $(this),
            textbox = $this.parent().siblings('.textbox'),
            list = $this.closest('.feed-comment').find('ul'),
            data = $this.data();

        if (!textbox.html()) {
            DisplayCommentError("评论内容不能为空！")
            return;
        }

        data.postcontent = textbox.html();

        $.ajax('/Home/SaveFeedComment', {
            type: "POST",
            data: JSON.stringify(data),
            contentType: 'application/json',
            dataType: "html",
            success: function (result) {
                result = $(result.trim());
                result.hide();
                list.prepend(result);
                if (list.find('.blank-image')) {
                    list.find('.blank-image').remove();
                }
                result.slideDown('slow');
                textbox.html('');
                UpdateAnswerCmtCount(data.answerid, true);
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayCommentError(json.errorMessage);
            }
        });
    });

    $(document).on('click', '.feed-comment a[id^="btnCmtRply"]', function (e) {
        e.preventDefault();

        var $this = $(this),
            data = $this.data(),
            placeholder = $this.closest('.feed-comment').find('.placeholder'),
            textbox = $this.closest('.feed-comment').find('.textbox'),
            submitBtn = $this.closest('.feed-comment').find('.text .btn'),
            prefixText = '回复 ' + data.targetname + ':&nbsp;';

        textbox.focus();
        textbox.html(prefixText);
        submitBtn.data('notifyuserid', data.targetid);
        placeholder.hide();
    });

    $(document).on('focus', '.feed-comment .textbox', function (e) {
        $(this).closest('.input').css('opacity', 1);
    });

    $(document).on('focusout', '.feed-comment .textbox', function () {
        $(this).closest('.input').removeAttr('style');
    });
});