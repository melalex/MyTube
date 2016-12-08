$(function () {
    $.ajaxSetup({ cache: false });
    $("#signIn").click(showSignInModal);
})

function showSignInModal(event)
{
    event.preventDefault();
    $.get(this.href, function (data) {
        $('#dialogContent').html(data);
        var modDialog = $('#modDialog');
        modDialog.modal('show');
        modDialog.modal({
            backdrop: 'static',
            keyboard: false  // to prevent closing with Esc button (if you want this too)
        });
        $("#loginBtn").click(login);
    });
}

function login(event)
{
    event.preventDefault();
    $.ajax({
        url: this.href,
        type: "POST",
        data: $("#loginForm").serialize(),
        datatype: "json",
        success: function (result) {
            $("#dialogContent").html(result);
        }
    })
}