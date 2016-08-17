function OnFollowBegin() {
    var loadingTab = $('#divInfoLoading').find('strong');
    loadingTab.text('正在提交申请，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
}

function OnFollowSuccess() {
    $('#btnFollow').toggle('fast');
    $('#btnUnfollow').toggle('fast');
    $('#btnWriteLetter').removeClass('disabled').text('发消息');

    DisplaySuccessInfo('恭喜，您已成功关注用户！')

    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount + 1);
}

function OnFollowComplete() { }

function OnFollowFailure() {
    DisplayErrorInfo('申请关注出现问题，请您查看……');
}

function OnUnfollowBegin() {
    var loadingTab = $('#divInfoLoading').find('strong');
    loadingTab.text('正在提交申请，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
}

function OnUnfollowSuccess() {
    $('#btnFollow').toggle('fast');
    $('#btnUnfollow').toggle('fast');
    $('#btnWriteLetter').addClass('disabled').text('关注后发消息');

    DisplaySuccessInfo('恭喜，您已取消关注用户！')

    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount - 1);
}

function OnUnfollowComplete() { }

function OnUnfollowFailure() {
    DisplayErrorInfo('取消关注出现问题，请您查看……');
}