function LoadPopularVideos(url) {

}

function LoadSimilarVideos()
{
    var destination = $("#similarVideos");
    var url = destination.data("similar-request");
    destination.load(url);
}

function LoadComments() {
    var destination = $("#paginationDestination");
    var url = destination.data("pagination-request");
    destination.load(url, PaginationStartUp);
}

function PaginationStartUp(event)
{
    $("a[data-pagination-request]").click(function (e) {
        var url = $(this).data("pagination-request");
        $("#paginationDestination").load(url)
    })
}