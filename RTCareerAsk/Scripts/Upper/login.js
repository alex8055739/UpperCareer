$(document).ready(function () {
    $('#btnForget').click(function (e) {
        $.ajax({
            type: "POST",
            url: "/Account/ForgetPasswordForm",
            dataType: "html",
            success: function (response) {
                $('#divModalSm').children().html(response);
            }
        });
    });
});