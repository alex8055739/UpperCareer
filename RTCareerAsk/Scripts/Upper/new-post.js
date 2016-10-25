function OnPostBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交问题，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('#divModal').modal('hide');
}

function OnAnsBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交答案，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('#divAnsForm').collapse('toggle');
}

function OnCmtBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在提交评论，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('form').closest('.collapse').collapse('hide');
}

function OnPostSuccess() {
    DisplaySuccessInfo('恭喜，您已成功发布问题！')

    if ($('.tab-content').length > 0 && $('.tab-sorting:contains("最新")').length > 0) {
        $('.tab-content:contains("问题")').parent().trigger('click');
        $('.tab-sorting:contains("最新")').parent().trigger('click');
    }
}

function OnAnsSuccess() {
    DisplaySuccessInfo('恭喜，您已成功发布答案！')

    $('#btnSubmitAns').addClass('disabled');
    $('#btnWriteAnswer').fadeOut('slow');
}

function OnCmtSuccess(ansId) {
    DisplaySuccessInfo('恭喜，您已成功发布评论！')

    var element = $('#divCmtList' + ansId + '> .box').first(),
        elemHeight = element.height(),
        winHeight = $(window).height(),
        offset = $('#divCmtList' + ansId).offset().top + elemHeight / 2 - winHeight / 2;

    element.hide().fadeIn('slow');

    $('html,body').animate({ scrollTop: offset }, 600);
}

function OnPostComplete() {
    //alert('提问操作完成！');
}

function OnAnsComplete() {
    //alert('回答操作完成！');
}

function OnCmtComplete() {
    UpdateCmtCount();
}

function OnPostFailure(e) {
    DisplayErrorInfo('提交问题出现问题，请您查看……');
}

function OnAnsFailure(e) {
    DisplayErrorInfo('提交答案出现问题，请您查看……');
}

function OnCmtFailure(e) {
    DisplayErrorInfo('提交评论出现问题，请您查看……');
}

function OnLetterBegin() {
    var loadingTab = $('#divInfoLoading').find('p');
    loadingTab.text('正在发送信件，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
    $('#divModal').modal('hide');
}

function OnLetterSuccess() {
    DisplaySuccessInfo('私信发送成功！')
}

function OnLetterComplete() {

}

function OnLetterFailure() {
    DisplayErrorInfo('发送信件出现问题，请您查看……');
}
