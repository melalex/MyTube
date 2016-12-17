function LoadSimilarVideos() {
    var destination = $("#similarVideos");
    var url = destination.data("similar-request");
    destination.load(url);
}

function LoadComments() {
    var destination = $("#paginationDestination");
    var url = destination.data("pagination-request");
    destination.load(url, PaginationStartUp);
}

function AddComment(event) {
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

var currentStatus = 0;

function Like(event) {
    event.preventDefault();
    if (currentStatus != 1) {
        if (currentStatus == 2) {
            var dislikes = parseInt($("#dislikes").text());
            $("#dislikes").text(dislikes - 1);
        }
        currentStatus = 1;
        var likes = parseInt($("#likes").text());
        $("#likes").text(likes + 1);

        $.ajax({
            url: this.href,
            type: "POST",
            data: $("#likeForm").serialize(),
            datatype: "json",
        })
    }
}

function Dislike(event) {
    event.preventDefault();
    if (currentStatus != 2) {
        if (currentStatus == 1) {
            var likes = parseInt($("#likes").text());
            $("#likes").text(likes - 1);
        }
        currentStatus = 2;
        var dislikes = parseInt($("#dislikes").text());
        $("#dislikes").text(dislikes + 1);

        $.ajax({
            url: this.href,
            type: "POST",
            data: $("#dislikeForm").serialize(),
            datatype: "json",
        })
    }
}

$(function () {
    SubscribtionStartUp();
    LoadComments();
    LoadSimilarVideos();
    $("#sendComment").click(AddComment);
    var avaterSrc = $("#channelAvatar").attr("src");
    $("#userAvatar").attr("src", avaterSrc);
    $("#likeBtn").click(Like);
    $("#dislikeBtn").click(Dislike);
});