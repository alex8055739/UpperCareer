function OnFollowBegin() {
    var loadingTab = $('#divInfoLoading').find('strong');
    loadingTab.text('正在提交申请，请您耐心等待');
    loadingTab.append('<div class="progress progress-striped active"><div class="progress-bar progress-bar-info" style="width: 100%"></div></div>');
}

function OnFollowSuccess() {
    $('#btnFollow').toggle('fast');
    $('#btnUnfollow').toggle('fast');
    $('#btnWriteLetter').removeClass('disabled').text('发消息');

    var loadingTab = $('#divInfoSuccess').find('strong');
    loadingTab.text('恭喜，您已成功关注用户！');
    loadingTab.closest('.alert-tag').toggle('slow');
    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount + 1);
}

function OnFollowComplete() { }

function OnFollowFailure() {
    var errorTab = $('#divInfoError');
    errorTab.find('strong').html('申请关注出现问题，请您查看……');
    errorTab.toggle('slow');
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

    var loadingTab = $('#divInfoSuccess').find('strong');
    loadingTab.text('成功，您已取消关注用户！');
    loadingTab.closest('.alert-tag').toggle('slow');
    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount - 1);
}

function OnUnfollowComplete() { }

function OnUnfollowFailure() {
    var errorTab = $('#divInfoError');
    errorTab.find('strong').html('取消关注出现问题，请您查看……');
    errorTab.toggle('slow');
}