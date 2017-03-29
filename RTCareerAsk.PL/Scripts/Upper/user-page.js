﻿function OnUpdateContentFailure(xhr) {
    var json = $.parseJSON(xhr.responseText);
    DisplayErrorInfo(json.errorMessage);
}

function OnUpdateContentComplete() {
    BindScrollPaging();
}

function AfterUserAnswersLoad() {
    Resize();
    $('li>.list-group-item-text').uppershorten();
}

function BindScrollPaging() {
    var contentTypeId = $('ul.list-group').data('type'),
        isOnRecords = contentTypeId == 1 || contentTypeId == 2;

    if (isOnRecords) {
        if (contentTypeId == 1) {
            $(document).ready(function () {
                $('ul.list-group').upperscrollpaging(loadUserRecords, {
                    itemSelector: 'li.list-group-item',
                    contentType: $('ul.list-group').data('type'),
                    targetId: $('ul.list-group').data('target')
                });
            });
        }
        else {
            AfterUserAnswersLoad();

            $('ul.list-group').upperscrollpaging(loadUserRecords, {
                itemSelector: 'li.list-group-item',
                contentType: $('ul.list-group').data('type'),
                targetId: $('ul.list-group').data('target'),
                postAction: AfterUserAnswersLoad
            });
        }
    }
    else {
        $('ul.list-group').upperscrollpaging(loadFollowInfo, {
            itemSelector: 'li.list-group-item',
            contentType: $('ul.list-group').data('type'),
            targetId: $('ul.list-group').data('target')
        });
    }
}

$(document).ready(function () {
    $('.upper-tab').uppertabs();

    BindScrollPaging();

    $(document).on('click', '.user-detail .action > button', function (e) {
        e.preventDefault();

        var $this = $(this),
            isFollow = $this.hasClass('follow'),
            url = isFollow ? '/User/FollowUser' : '/User/UnfollowUser',
            currentClass = isFollow ? 'follow' : 'unfollow',
            currentButton = isFollow ? 'btn-success' : 'btn-default',
            oppositeClass = isFollow ? 'unfollow' : 'follow',
            oppositeButton = isFollow ? 'btn-default' : 'btn-success',
            updatedHtml = isFollow ? '<span class="glyphicon glyphicon-eye-close"></span>&nbsp;取消关注' : '<span class="glyphicon glyphicon-eye-open"></span>&nbsp;关注用户',
            newFollowerCount = isFollow ? parseInt($('#spnFollowerCount').html()) + 1 : parseInt($('#spnFollowerCount').html()) - 1,
            newLetterBtnText = isFollow ? '<span class="glyphicon glyphicon-send"></span>&nbsp;发消息' : '<span class="glyphicon glyphicon-send"></span>&nbsp;关注后发消息',
            disableClass = 'disabled',
            targetId = $this.data('target')

        $.ajax(url, {
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ id: targetId }),
            beforeSend: function () {
                $this.addClass(disableClass)
            },
            success: function () {
                $this.removeClass(currentClass).removeClass(currentButton).addClass(oppositeClass).addClass(oppositeButton).html(updatedHtml);
                $('#spnFollowerCount').html(newFollowerCount);
                $('#btnWriteLetter').toggleClass(disableClass).html(newLetterBtnText)
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            },
            complete: function () {
                $this.removeClass(disableClass)
            }
        })
    });

    //$(document).on('click', '.user-list button', function (e) {
    //    e.preventDefault();

    //    var $this = $(this),
    //        isFollow = $this.hasClass('follow'),
    //        url = isFollow ? '/User/FollowUser' : '/User/UnfollowUser',
    //        currentClass = isFollow ? 'follow' : 'unfollow',
    //        currentButton = isFollow ? 'btn-success' : 'btn-default',
    //        oppositeClass = isFollow ? 'unfollow' : 'follow',
    //        oppositeButton = isFollow ? 'btn-default' : 'btn-success',
    //        updatedHtml = isFollow ? '<span class="glyphicon glyphicon-eye-close"></span>&nbsp;取消关注' : '<span class="glyphicon glyphicon-eye-open"></span>&nbsp;关注用户',
    //        disableClass = 'disabled',
    //        targetId = $this.data('target')

    //    $.ajax(url, {
    //        method: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify({ id: targetId }),
    //        beforeSend: function () {
    //            $this.addClass(disableClass)
    //        },
    //        success: function () {
    //            $this.removeClass(currentClass).removeClass(currentButton).addClass(oppositeClass).addClass(oppositeButton).html(updatedHtml);
    //        },
    //        error: function (xhr) {
    //            var json = $.parseJSON(xhr.responseText);
    //            DisplayErrorInfo((isFollow ? '申请关注出现问题: ' : '取消关注出现问题: ') + json.errorMessage);
    //        },
    //        complete: function () {
    //            $this.removeClass(disableClass)
    //        }
    //    })
    //});

    $(document).on('click', '.user-detail .action > a.recommand', function (e) {
        e.preventDefault();

        var modal = $('#divModalSm'),
            data = $(this).data();

        $.ajax('/User/CreateUserRecommandModal', {
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            dataType: 'html',
            success: function (result) {
                modal.children().html(result.trim());
                modal.modal('show');
            },
            error: function (xhr) {
                var json = $.parseJSON(xhr.responseText);
                DisplayErrorInfo(json.errorMessage);
            }
        });
    });

    $(document).on('click', '.modal-footer>button.recommand', function (e) {
        e.preventDefault();

        var modal = $('#divModalSm'),
            data = $(this).data(),
            textArea = modal.find('#txtIntro');

        if (textArea.val()) {
            data.intro = textArea.val();

            $.ajax('/User/RecommandUser', {
                type: 'POST',
                data: JSON.stringify(data),
                contentType: 'application/json',
                beforeSend: function () {
                    modal.addClass('loading');
                },
                success: function () {
                    DisplaySuccessInfo('推荐成功！');
                },
                error: function (xhr) {
                    var json = $.parseJSON(xhr.responseText);
                    DisplayErrorInfo(json.errorMessage);
                },
                complete: function () {
                    modal.removeClass('loading');
                    modal.modal('hide');
                }
            })

        }
        else {
            var errorInfo = $(document.createElement('p')).text('您还没有输入用户介绍！').css('color', '#e00303');
            textArea.before(errorInfo);
        }
    });
});

//function OnFollowBegin() {
//    $('#btnFollow').addClass('disabled').text('关注中...');
//}

//function OnFollowSuccess() {
//    $('#btnFollow').toggle();
//    $('#btnUnfollow').toggle();
//    $('#btnWriteLetter').removeClass('disabled').html('<span class="glyphicon glyphicon-send"></span>&nbsp;发消息');

//    //DisplaySuccessInfo('恭喜，您已成功关注用户！')

//    var newFollowerCount = parseInt($('#spnFollowerCount').html());
//    $('#spnFollowerCount').html(newFollowerCount + 1);
//}

//function OnFollowComplete() {
//    $('#btnFollow').removeClass('disabled').text('关注TA');
//}

//function OnFollowFailure() {
//    DisplayErrorInfo('申请关注出现问题，请您查看……');
//}

//function OnUnfollowBegin() {
//    $('#btnUnfollow').addClass('disabled').text('取关中...');
//}

//function OnUnfollowSuccess() {
//    $('#btnFollow').toggle();
//    $('#btnUnfollow').toggle();
//    $('#btnWriteLetter').addClass('disabled').html('<span class="glyphicon glyphicon-send"></span>&nbsp;关注后发消息');

//    //DisplaySuccessInfo('恭喜，您已取消关注用户！')

//    var newFollowerCount = parseInt($('#spnFollowerCount').html());
//    $('#spnFollowerCount').html(newFollowerCount - 1);
//}

//function OnUnfollowComplete() {
//    $('#btnUnfollow').removeClass('disabled').text('取消关注');
//}

//function OnUnfollowFailure(xhr) {
//    DisplayErrorInfo('取消关注出现问题，请您查看……');
//}
