function OnFollowBegin() {
    $('#btnFollow').addClass('disabled').text('关注中...');
}

function OnFollowSuccess() {
    $('#btnFollow').toggle();
    $('#btnUnfollow').toggle();
    $('#btnWriteLetter').removeClass('disabled').text('发消息');

    DisplaySuccessInfo('恭喜，您已成功关注用户！')

    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount + 1);
}

function OnFollowComplete() {
    $('#btnFollow').removeClass('disabled').text('关注TA');
}

function OnFollowFailure() {
    DisplayErrorInfo('申请关注出现问题，请您查看……');
}

function OnUnfollowBegin() {
    $('#btnUnfollow').addClass('disabled').text('取关中...');
}

function OnUnfollowSuccess() {
    $('#btnFollow').toggle();
    $('#btnUnfollow').toggle();
    $('#btnWriteLetter').addClass('disabled').text('关注后发消息');

    DisplaySuccessInfo('恭喜，您已取消关注用户！')

    var newFollowerCount = parseInt($('#spnFollowerCount').html());
    $('#spnFollowerCount').html(newFollowerCount - 1);
}

function OnUnfollowComplete() {
    $('#btnUnfollow').removeClass('disabled').text('取消关注');
}

function OnUnfollowFailure() {
    DisplayErrorInfo('取消关注出现问题，请您查看……');
}

function OnUpdateContentComplete() { }

$(document).ready(function () {
    $('.upper-tab').uppertabs();
});