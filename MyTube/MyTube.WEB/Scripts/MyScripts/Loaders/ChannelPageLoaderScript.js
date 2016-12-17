function LoadUploadedVideos() {
    var destination = $("#paginationDestination");
    var url = destination.data("pagination-request");
    destination.load(url, PaginationStartUp);
}

$(function () {
    LoadUploadedVideos();
    SubscribtionStartUp();
});