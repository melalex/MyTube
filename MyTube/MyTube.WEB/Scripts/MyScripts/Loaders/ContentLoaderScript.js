function PaginationStartUp(event)
{
    $("a[data-pagination-request]").click(function (e) {
        var url = $(this).data("pagination-request");
        $("#paginationDestination").load(url)
    })
}

function Subscribe(event) {
    event.preventDefault();
    $.ajax({
        url: this.href,
        type: "POST",
        data: $("#subscribeForm").serialize(),
        datatype: "json",
        success: function (result) {
            $(this).text("Unsubscribe");
            var subscribersCountTag = $("#subscribersCount");
            var subscribersCount = parseInt(subscribersCountTag.data("subscribers-count"));
            subscribersCount++;
            subscribersCountTag.data("subscribers-count", subscribersCount);
            subscribersCountTag.text(subscribersCount + " subscribers");
            $(this).unbind("click").click(Unsubscribe);
        }
    })
}

function Unsubscribe(event) {
    event.preventDefault();
    $.ajax({
        url: this.href,
        type: "POST",
        data: $("#unsubscribeForm").serialize(),
        datatype: "json",
        success: function (result) {
            $(this).text("Subscribe");
            var subscribersCountTag = $("#subscribersCount");
            var subscribersCount = parseInt(subscribersCountTag.data("subscribers-count"));
            subscribersCount--;
            subscribersCountTag.data("subscribers-count", subscribersCount);
            subscribersCountTag.text(subscribersCount + " subscribers");
            $(this).unbind("click").click(Subscribe);
        }
    })
}

function SubscribtionStartUp() {
    $("#subscribe").click(Subscribe);
    $("#unsubscribe").click(Unsubscribe);
}