function OnMsgBegin() {
    $('#divMsgBody').empty();
}

function OnMsgSuccess() {
    var msgId = $('#hdnMsgID').val();
    var newBadge = $('#lnkMsg' + msgId).closest('li').find('span');
    if (newBadge.length) {
        newBadge.remove();
        var newCount = $("span:contains('new').badge").length
        UpdateNewMsgCount();
    }
}

function OnMsgComplete() {
}

function OnMsgFailure(e) {
    var errorTab = $('#divInfoError');
    errorTab.find('strong').html('加载消息出现问题，请您查看……');
    errorTab.toggle('slow');
}

function OnDeleteBegin() {
    $('#divMsgBody').empty();
    $('#imgMsgLoading').show();
}

function OnDeleteSuccess(e) {
    var SuccessTab = $('#divInfoSuccess').find('strong');
    SuccessTab.text('您已成功删除消息！');
    SuccessTab.closest('.alert-tag').toggle('slow');

    var msgLink = $('#lnkMsg' + e).closest('li');
    msgLink.hide(500);
    $('#divMsgBody').html('<div>请您从列表中选择要阅读的消息</div>');
    //msgLink.remove();
}

function OnDeleteComplete() {
    $('#imgMsgLoading').hide();
}

function OnDeleteFailure(e) {
    var errorTab = $('#divInfoError');
    errorTab.find('strong').html(e);
    errorTab.toggle('slow');
}

$(document).ready(function () {
    $(document.body).on('click', '#btnDeleteMsg', function () {

        var targetId = $('#hdnMsgID').val();
        $.ajax({
            url: '/Message/DeleteMessage',
            data: { targetId: targetId },
            type: 'POST',
            beforeSend: function () {
                OnDeleteBegin();
            },
            success: function () {
                OnDeleteSuccess(targetId);
            },
            error: function (e) {
                alert('error');
                OnDeleteFailure(e.responseText);
            },
            complete: function () {
                OnDeleteComplete();
            }
        });
    })
});