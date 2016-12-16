function AddComment(event)
{
    event.preventDefault();
    var textArea = $("#comment");
    $.ajax({
        url: this.href,
        type: "POST",
        data: $("#commentForm").serialize(),
        datatype: "json",
        success: function (result) {
            $("#comment").val("");
            LoadComments();
        }
    })
}

$(function () {
    LoadComments();
    LoadSimilarVideos();
    $("#sendComment").click(AddComment);
});