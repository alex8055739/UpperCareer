function OnMsgBegin() {
    $('#divMsgBody').empty();
}

function OnMsgSuccess() {
    var msgId = $('#hdnMsgID').val();
    var newBadge = $('#lnkMsg' + msgId).closest('li').find('span');
    if (newBadge.length) {
        newBadge.remove();
        UpdateNewMsgCount();
    }
}

function OnMsgComplete() {
}

function OnMsgFailure(e) {
    DisplayErrorInfo('加载消息出现问题，请您查看……');
}

function OnDeleteBegin() {
    $('#divMsgBody').empty();
    $('#divLoader').show();
}

function OnDeleteSuccess(e) {
    DisplaySuccessInfo('您已成功删除消息！')

    var msgLink = $('#lnkMsg' + e).closest('li');
    msgLink.fadeOut(500);
    $('#divMsgBody').html('<div>请您从列表中选择要阅读的消息</div>');
}

function OnDeleteComplete() {
    $('#divLoader').hide();
}

function OnDeleteFailure(e) {
    DisplayErrorInfo(e.responseText);
}

$(document).ready(function () {
    $(document.body).on('click', '#btnDeleteMsg', function (e) {
        e.preventDefault();

        var data = $(this).data();

        data.url = deleteMessage;

        RequestForDelete(data);
    })

    $(document).on('delSuccess', function (e, data) {
        OnDeleteSuccess(data.id);
    })
});