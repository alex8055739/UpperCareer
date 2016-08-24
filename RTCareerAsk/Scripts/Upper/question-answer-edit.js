function UpdateContent(isQuestion, id, content) {
    var data = new Object();
    data.isQuestion = isQuestion;
    data.id = id;
    data.content = content;

    $.ajax(updateContent, {
        method: 'POST',
        async: false,
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function () {
            DisplaySuccessInfo('内容更新成功！');
            return true;
        },
        error: function () {
            DisplayErrorInfo('内容更新出现问题……');
            return false;
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
            var result = UpdateContent('true', id, data);

            if (result) {
                qContent.removeAttr('contenteditable');
                CKEDITOR.instances[qContent.attr('id')].destroy();
                $(this).removeClass('btn-success').removeClass('disabled').addClass('btn-warning').text('编辑问题');
            }
            else {
                $(this).removeClass('disabled').text('保存修改');
            }
        }
    });

    $(document).on('click', 'a[id^="btnAnsDel"]', function () {
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
