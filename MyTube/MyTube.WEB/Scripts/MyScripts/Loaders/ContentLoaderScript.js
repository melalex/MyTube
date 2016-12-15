function LoadPopularVideos(url) {

}

function LoadSimilarVideos()
{
    var destination = $("#similarVideos");
    var url = destination.data("similar-request");
    destination.load(url);
}