$(function () {
    $.ajaxSetup({ cache: false });
    $("#signUp").click(showSignUpModal);
})

function showSignUpModal(event) {
    event.preventDefault();
    $.get(this.href, function (data) {
        $('#dialogContent').html(data);
        $("#registerBtn").click(register);
        var modDialog = $('#modDialog');
        modDialog.modal('show');
        modDialog.modal({
            backdrop: 'static',
            keyboard: false  // to prevent closing with Esc button (if you want this too)
        });
    });
}

function register(event) {
    event.preventDefault();
    $.ajax({
        url: this.href,
        type: "POST",
        data: $("#registerForm").serialize(),
        datatype: "json",
        success: function (result) {
            $("#dialogContent").html(result);
        }
    })
}