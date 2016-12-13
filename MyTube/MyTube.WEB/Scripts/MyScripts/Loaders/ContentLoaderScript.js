function LoadProfileZone() {
    var profileZone = $("#profileZone");
    var url = profileZone.data("profilezone-request-url");
    profileZone.load(url);
}

function LoadProfileZoneWithCallbach(callback) {
    var profileZone = $("#profileZone");
    var url = profileZone.data("profilezone-request-url");
    profileZone.load(url, callback);
}

function LoadPopularVideos(url) {

}