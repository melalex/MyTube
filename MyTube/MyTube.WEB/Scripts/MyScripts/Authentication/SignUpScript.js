$(function () {
    $.ajaxSetup({ cache: false });
    $("#signUp").click(showSignUpModal);
})

function showSignUpModal(event) {
    event.preventDefault();
    $.get(this.href, function (data) {
        $('#dialogContent').html(data);
        var modDialog = $('#modDialog');
        modDialog.modal('show');
        modDialog.modal({
            backdrop: 'static',
            keyboard: false  // to prevent closing with Esc button (if you want this too)
        });
    });
}