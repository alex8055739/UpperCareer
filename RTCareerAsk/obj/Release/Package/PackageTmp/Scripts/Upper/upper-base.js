//Fix compatibility issue with CKEditor in a Bootstrap Modal.
$.fn.modal.Constructor.prototype.enforceFocus = function () {
    $(document)
        .off('focusin.bs.modal') // guard against infinite focus loop
        .on('focusin.bs.modal', $.proxy(function (e) {
            if (
                this.$element[0] !== e.target && !this.$element.has(e.target).length
                // CKEditor compatibility fix start.
                && !$(e.target).closest('.cke_dialog, .cke').length
                // CKEditor compatibility fix end.
            ) {
                this.$element.trigger('focus');
            }
        }, this));
};
//Trigger Bootstrap tooltip when page load.
$(function () {
    TriggerTooltip();
})
//Function to trigger Bootstrap tooltip.
function TriggerTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}

function UpdateNewMsgCount() {
    $.ajax({
        url: updateMsgCount,
        type: 'POST',
        success: function (result) {
            $('#divNavBar').html(result);
        },
        error: function (e) {
            alert(e.responseText);
        }
    });
}

$(document).ready(function () {
    $('button.close').click(function () {
        $(this).closest('.alert-tag').toggle('slow');
    })

    $('#btnPostQuestion').click(function (e) {
        $.ajax({
            url: loadQuestionForm,
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    });

    $(document.body).on('click', '#btnWriteLetter', function () {
        var targetId = $('#hdnSender').val();

        $.ajax({
            url: '/Message/CreateLetterForm',
            data: { targetId: targetId },
            dataType: 'html',
            type: 'POST',
            success: function (result) {
                $('#divModal').children().html(result);
            },
            error: function (e) {
                alert(e.responseText);
            }
        });
    })
});
