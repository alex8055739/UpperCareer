function UpdateContent(isQuestion, id, content) {
    var data = new Object();
    data.isQuestion = isQuestion;
    data.id = id;
    data.content = content;

    var triggerTarget = isQuestion == 'true' ? $('#btnEditQuestion') : $('#btnAnsEdt' + id);

    $.ajax(updateContent, {
        method: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json",
        beforeSend: function () {
            triggerTarget.addClass('not-active').text('保存更新中……');
        },
        success: function () {
            DisplaySuccessInfo('内容更新成功！');
            triggerTarget.trigger('updateSuccess');
        },
        error: function () {
            DisplayErrorInfo('内容更新出现问题……');
            triggerTarget.trigger('updateError');
        }
    });
}

function ActivateCmtForm(ansId, ntfyId, prefixTxt, appendTarget) {
    var cmtForm = $('#divCmtForm' + ansId);

    cmtForm.find('textarea').val(prefixTxt);
    cmtForm.find('input[name="NotifyUserID"]').val(ntfyId);
    appendTarget.after(cmtForm);
    cmtForm.collapse('show').focus();
}

function UpdateAnswerCount() {
    var header = $('.answer-detail').find('.header').first(),
        count = header.parent().siblings('.body').length - 1,
        h3 = header.find('h3').first(),
        numberBadge = header.find('.marked-number').first();

    if (count > 0) {
        numberBadge.text(count);
    }
    else {
        h3.text('很抱歉，还没有人回答这个问题');
    }
}

$(document).ready(function () {
    CKEDITOR.disableAutoInline = true;

    $('#btnEditQuestion').click(function () {
        var qContent = $('div[id^="divQContent"]');

        if (!qContent.attr('contenteditable')) {
            qContent.attr('contenteditable', true);
            CKEDITOR.inline(qContent.attr('id'));
            qContent.focus();

            $(this).text('保存修改').css('color', 'red');
        }
        else {
            var data = CKEDITOR.instances[qContent.attr('id')].getData();
            var id = qContent.attr('id').replace('divQContent', '');
            UpdateContent('true', id, data);
        }
    });

    $(document).on('click', 'a[id^="btnAnsEdt"]', function (e) {
        e.preventDefault();

        CKEDITOR.disableAutoInline = true;

        var id = $(this).attr('id').replace('btnAnsEdt', '');
        var ansTxt = $('#divAnsTxt' + id);

        if (!ansTxt.attr('contenteditable')) {
            ansTxt.attr('contenteditable', true);
            CKEDITOR.inline(ansTxt.attr('id'),
                {
                    wordcount: {
                        showWordCount: false,
                        showCharCount: true,
                        maxCharCount: 5000
                    }
                });
            ansTxt.focus();
            $(this).text('保存修改').css('color', 'red');
        }
        else {
            var data = CKEDITOR.instances[ansTxt.attr('id')].getData();
            UpdateContent('false', id, data);
        }
    });

    $('#btnEditQuestion').on('updateSuccess', function () {
        var qContent = $('div[id^="divQContent"]');

        qContent.removeAttr('contenteditable');
        CKEDITOR.instances[qContent.attr('id')].destroy();
        $(this).text('编辑问题').removeAttr('style').removeClass('not-active');
    });

    $(document).on('updateSuccess', 'a[id^="btnAnsEdt"]', function () {
        $(this).removeAttr('style').removeClass('not-active').text('编辑');

        var ansTxt = $('#divAnsTxt' + $(this).attr('id').replace('btnAnsEdt', ''));
        ansTxt.removeAttr('contenteditable');
        CKEDITOR.instances[ansTxt.attr('id')].destroy();
    });

    $('#btnEditQuestion').on('updateError', function () {
        $(this).removeClass('not-active').text('保存修改');
    });

    $(document).on('updateError', 'a[id^="btnAnsEdt"]', function () {
        $(this).removeClass('not-active').text('保存修改');
    });

    $(document).on('click', 'a[id^="btnAnsDel"]', function (e) {
        e.preventDefault();

        if (confirm('确认删除答案？')) {
            var ansBlock = $(this).closest('div[class="body"]'),
                data = new Object();

            data.ansId = $(this).attr('id').replace('btnAnsDel', '');

            $.ajax(deleteAnswer, {
                method: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                success: function () {
                    DisplaySuccessInfo('成功删除回答！');
                    ansBlock.fadeOut('slow');
                    $('#btnWriteAnswer').removeClass('not-active');
                    //$('#btnWriteAnswer').parent().fadeIn('slow');
                    $('#btnSubmitAns').removeClass('disabled');
                    UpdateAnswerCount();
                },
                error: function () {
                    DisplayErrorInfo('删除操作出现问题……');
                }
            });
        }
    });

    $(document).on('click', 'a[id^="btnCmtDel"]', function (e) {
        e.preventDefault();

        if (confirm('确认删除评论？')) {
            var cmtId = $(this).attr('id').replace('btnCmtDel', ''),
                data = new Object(),
                $this = $(this),
                cmtBlock = $('#blkCmt' + cmtId);

            data.cmtId = cmtId;

            $.ajax(deleteComment, {
                type: "POST",
                data: JSON.stringify(data),
                contentType: 'application/json',
                success: function () {
                    DisplaySuccessInfo('成功删除评论！');
                    cmtBlock.fadeOut('slow');
                    if ($this.closest('.body').find('.CmtCount > span').html()) {
                        var count = parseInt($this.closest('.body').find('.CmtCount > span').html().replace('(', '').replace(')', '')) - 1;
                        $this.closest('.body').find('.CmtCount > span').html('(' + count + ')');
                    }
                    else if ($('span.cmt-count')) {
                        var count = $('span.cmt-count').parent().siblings('div[id^="divCmtList"]').first().children('div[id^="blkCmt"]:visible').length - 1;
                        $('span.cmt-count').text(count);
                    }
                },
                error: function () {
                    DisplayErrorInfo('删除操作出现问题……');
                }
            });
        }
    })

    $(document).on('click', 'a[id^="btnCmtRply"]', function (e) {
        e.preventDefault();
        var ansId = $(this).closest('div[id^="divCmtList"]').attr('id').replace('divCmtList', '');

        if ($(this).text() == '收起回复') {
            $('#divCmtForm' + ansId).collapse('hide');
            $(this).text('回复');
        }
        else {
            $(this).closest('div[id^="divCmtList"]').find('a[id^="btnCmtRply"]').text('回复');
            $('#btnWrtCmt' + ansId).text('评一下');

            var replyToId = $(this).attr('id').replace('btnCmtRply', '');
            var prefixText = "回复 " + $('#aCmtBy' + replyToId).text() + "： ";
            var appendTarget = $(this).closest('.box');

            ActivateCmtForm(ansId, replyToId, prefixText, appendTarget);
            $(this).text('收起回复');
        }
    });

    $(document).on('click', 'a[id^="btnWrtCmt"]', function (e) {
        e.preventDefault();
        var ansId = $(this).attr('id').replace('btnWrtCmt', '');

        if ($(this).text() == '收起评论') {
            $('#divCmtForm' + ansId).collapse('hide');
            $(this).text('评一下');
        }
        else {
            var appendTarget = $('#divAns' + ansId),
                replyToId = $('#hdnAnsBy' + ansId).val(),
                prefixText = '';

            ActivateCmtForm(ansId, replyToId, prefixText, appendTarget);
            $('#divCmtList' + ansId).find('a[id^="btnCmtRply"]').text('回复');
            $(this).text('收起评论');
        }
    });

    $(document).on('click', 'div[id^="divAnsVote"] a', function (e) {
        e.preventDefault();
        var $this = $(this),
            wrap = $this.closest('div[id^="divAnsVote"]'),
            isLike = $this.html() == "赞",
            isUpdate = !$this.hasClass('new'),
            data = new Object(),
            valueChange = (isUpdate ? 2 : 1) * (isLike ? 1 : -1);

        data.TargetID = wrap.attr('id').replace('divAnsVote', '');
        data.Type = 1;
        data.IsLike = isLike;
        data.IsUpdate = isUpdate;

        $.ajax('/Question/SaveVote', {
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                wrap.find('a').addClass('not-active');
            },
            success: function () {
                wrap.find('.new').removeClass('new');
                wrap.find('#spnVoteDiff').text(parseInt(wrap.find('#spnVoteDiff').text()) + valueChange);
                wrap.find(isLike ? 'a:last' : 'a:first').removeClass('not-active');
            },
            error: function () {
                DisplayErrorInfo('投票操作出现问题……');
                wrap.find(isLike ? 'a:first' : 'a:last').removeClass('not-active');
            }
        });
    });

    $(document).on('click', '.question-detail .action .like', function (e) {
        e.preventDefault();

        var $this = $(this),
            classNonActive = 'not-active',
            classNew = 'new',
            classIndex = 'count',
            classButton = 'like',
            wrap = $this.parent(),
            opposite = $this.siblings('.' + classButton),
            isLike = $this.data('islike') == true,
            isUpdate = !$this.hasClass(classNew),
            data = new Object();

        data.TargetID = $this.data('id');
        data.Type = $this.data('type');
        data.IsLike = isLike;
        data.IsUpdate = isUpdate;

        $.ajax({
            type: "POST",
            url: "/Question/SaveVote",
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                wrap.children('.' + classButton).addClass(classNonActive);
            },
            success: function () {
                wrap.children('.' + classNew).removeClass(classNew);
                opposite.removeClass(classNonActive);
                $this.find('.' + classIndex).html(parseInt($this.find('.' + classIndex).html()) + 1);
                if (isUpdate) {
                    opposite.find('.' + classIndex).html(parseInt(opposite.find('.' + classIndex).html()) - 1);
                }
            },
            error: function () {
                DisplayErrorInfo('投票操作出现问题……');
                if (isUpdate) {
                    $this.removeClass(classNonActive);
                }
                else {
                    wrap.children().removeClass(classNonActive);
                }
            }
        });
    });

    $(document).on('click', '.arrow', function (e) {
        e.preventDefault();

        var $this = $(this),
            classNonActive = 'not-active',
            classNew = 'new',
            classIndex = 'votes',
            classButton = 'arrow',
            wrap = $this.parent(),
            opposite = $this.siblings('.' + classButton),
            isLike = $this.children('a').hasClass('up'),
            isUpdate = !$this.hasClass(classNew),
            data = new Object();

        data.TargetID = wrap.data('id');
        data.Type = wrap.data('type');
        data.IsLike = isLike;
        data.IsUpdate = isUpdate;

        $.ajax({
            type: "POST",
            url: "/Question/SaveVote",
            data: JSON.stringify(data),
            contentType: 'application/json',
            beforeSend: function () {
                wrap.children('.' + classButton).addClass(classNonActive);
            },
            success: function () {
                wrap.children('.' + classNew).removeClass(classNew);
                opposite.removeClass(classNonActive);
                $this.find('.' + classIndex).html(parseInt($this.find('.' + classIndex).html()) + 1);
                if (isUpdate) {
                    opposite.find('.' + classIndex).html(parseInt(opposite.find('.' + classIndex).html()) - 1);
                }
            },
            error: function () {
                DisplayErrorInfo('投票操作出现问题……');
                if (isUpdate) {
                    $this.removeClass(classNonActive);
                }
                else {
                    wrap.children().removeClass(classNonActive);
                }
            }
        });
    })

    $(document).on('submit', 'form[id^="formCmt"]', function (e) {
        e.preventDefault();

        var ansId = $(this).attr('id').replace('formCmt', '');
        $(this).find('textarea').val('');
        $('#divAns' + ansId).after($('#divCmtForm' + ansId));

        $('#btnWrtCmt' + ansId).text('评一下');
    });
});
