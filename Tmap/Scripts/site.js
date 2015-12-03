var map = new google.maps.Map(document.getElementById('map'), {
    zoom: 2,
    center: new google.maps.LatLng(0,0),
    disableDefaultUI: true,
    mapTypeId: google.maps.MapTypeId.ROADMAP,
    navigationControl: false,
    mapTypeControl: false,
    scaleControl: false,
    draggable: false,
    scrollwheel: false,
    panControl: false,
    maxZoom: 2,
    minZoom: 2
});

var infowindow = new google.maps.InfoWindow({
    disableAutoPan: true
});

var hub = $.connection.tweetHub;
var marker;
var content;

$.extend(hub.client, {
    

    sendTweet: function (lng,ltd,name,date,txt) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(ltd, lng),
            map: map
        });

        content = '<div align="left"><strong>Name: </strong> ' + name + '<br><strong>Date: </strong>' + date + '<br><strong>Tweet: </strong>' + txt + '</div>';
        google.maps.event.addListener(marker, 'mouseover', (function (marker, content, infowindow) {
            return function () {
                infowindow.setContent(content);
                infowindow.open(map, marker);
            };
        })(marker, content, infowindow));

        google.maps.event.addListener(marker, 'mouseout', function () {
            infowindow.close();
        });

        attachData(marker, content);
    }
});
function attachData(marker, secretMessage) {
    var infowindow = new google.maps.InfoWindow({
        content: secretMessage
    });

    marker.addListener('click', function () {
        infowindow.open(marker.get('map'), marker);
    });

}

$.connection.hub.start();