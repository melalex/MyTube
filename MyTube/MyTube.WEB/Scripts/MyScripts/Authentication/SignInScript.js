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
    });
    $("#signInBtn").click(showSignInModal);
}

