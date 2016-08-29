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
            triggerTarget.addClass('disabled').text('保存更新中……');
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

$(document).ready(function () {
    CKEDITOR.disableAutoInline = true;

    $('#btnEditQuestion').click(function () {
        var qContent = $('div[id^="divQContent"]');

        if (!qContent.attr('contenteditable')) {
            qContent.attr('contenteditable', true);
            CKEDITOR.inline(qContent.attr('id'));
            qContent.focus();

            $(this).removeClass('btn-warning').addClass('btn-success').text('保存修改');
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
            $(this).text('保存修改').attr('style', 'color:red');
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
        $(this).removeClass('btn-success').removeClass('disabled').addClass('btn-warning').text('编辑问题');
    });

    $(document).on('updateSuccess', 'a[id^="btnAnsEdt"]', function () {
        $(this).removeAttr('style').text('编辑');

        var ansTxt = $('#divAnsTxt' + $(this).attr('id').replace('btnAnsEdt', ''));
        ansTxt.removeAttr('contenteditable');
        CKEDITOR.instances[ansTxt.attr('id')].destroy();
    });

    $('#btnEditQuestion').on('updateError', function () {
        $(this).removeClass('disabled').text('保存修改');
    });

    $('a[id^="btnAnsEdt"]').on('updateError', function () {

    });

    $(document).on('click', 'a[id^="btnAnsDel"]', function (e) {
        e.preventDefault();

        if (confirm('确认删除答案？')) {
            var ansBlock = $(this).closest('div[class="answer"]');
            var data = new Object();
            data.ansId = $(this).attr('id').replace('btnAnsDel', '');

            $.ajax(deleteAnswer, {
                method: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                success: function () {
                    DisplaySuccessInfo('成功删除回答！');
                    ansBlock.fadeOut('slow');
                    $('#btnWriteAnswer').fadeIn('slow');
                    $('#btnSubmitAns').removeClass('disabled');
                },
                error: function () {
                    DisplayErrorInfo('删除操作出现问题……');
                }
            });
        }
    });
});
