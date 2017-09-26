var map = {};

function displayMediaType(type) {
    switch (type) {
    case 0: return 'audio';
    case 1: return 'video';
    case 2: return 'image';
    case 3: return 'text';
    }
    return 'unknown';
}

function showDistrictOverlay() {
    axios.get('district-high.json')
        .then(function (response) {
            const district = new google.maps.Polygon({
                map: map,
                paths: response.data,
                strokeColor: '#00F',
                strokeOpacity: 0.4,
                strokeWeight: 0.4,
                fillColor: '#00F',
                fillOpacity: 0.04,
                clickable: false
            });
            const bounds = new google.maps.LatLngBounds();
            response.data.forEach(function (point) {
                bounds.extend(point);
            }, this);
            map.fitBounds(bounds);
        });
}

function showRoute() {
    axios.get('route.json?' + new Date().getTime())
        .then(function (response) {
            response.data.forEach(function (section) {
                const info = new google.maps.InfoWindow({
                    content: '<h5>' + moment(section.date).format('l') + '</h5>'
                    + '<h6>' + section.description + '</h6>'
                });
                const line = new google.maps.Polyline({
                    map: map,
                    strokeColor: section.color,
                    path: section.path
                });
                line.addListener('click', function (e) {
                    info.open(map, line);
                    info.setPosition(e.latLng, e.latLng);
                });
            });
        });
}

function showMedia() {
    axios.get('media.json?' + new Date().getTime())
        .then(function (response) {
            response.data.forEach(function (media) {
                const info = new google.maps.InfoWindow({
                    content: '<h5>' + media.title + '</h5>'
                    + '<h6>' + moment(media.date).format('l LT') + '</h6>'
                    + (media.description ? '<p>' + media.description + '</p>' : '')
                    + '<iframe src=\'' + media.embedded_content + '\' frameborder=\'0\'></iframe>'
                });
                const marker = new google.maps.Marker({
                    map: map,
                    position: media.location,
                    title: media.title,
                    icon: {
                        url: displayMediaType(media.media_type) + '-poi-blue.png'
                    }
                });
                marker.addListener('click', function (e) {
                    info.open(map, marker);
                    info.setPosition(e.latLng, e.latLng);
                });
            });
        });
}

function showEvents() {
    axios.get('events.json?' + new Date().getTime())
        .then(function (response) {
            response.data.forEach(function (event_info) {
                const info = new google.maps.InfoWindow({
                    content: '<h5>' + event_info.title + '</h5>'
                    + '<h6>' + moment(event_info.date).format('l LT') + '</h6>'
                    + (event_info.description ? '<p>' + event_info.description + '</p>' : '')
                    + (event_info.link ? '<a href=\'' + event_info.link + '\'>More Info</a>' : '')
                });
                const marker = new google.maps.Marker({
                    map: map,
                    position: event_info.location,
                    title: event_info.title
                });
                marker.addListener('click', function (e) {
                    info.open(map, marker);
                    info.setPosition(e.latLng, e.latLng);
                });
            });
        });
}

var ian;
function updatePosition() {
    axios.get('ian.json?' + new Date().getTime())
        .then(function (response) {
            const location = response.data;
            const info = new google.maps.InfoWindow({
                content: '<h5>Ian\'s Location</h5>'
                + '<h6>' + moment(location.time).format('l LT') + '</h6>'
            });
            if (ian !== undefined) {
                ian.setMap(null);
            }
            ian = new google.maps.Marker({
                map: map,
                icon: {
                    url: 'running-poi-green.png'
                },
                position: location.position,
                title: 'Ian\'s location at ' + location.time,
                zIndex: 1000
            });
            ian.addListener('click', function (e) {
                info.open(map, ian);
                info.setPosition(e.latLng, e.latLng);
            });
        });
}

function watchIan() {
    updatePosition();
    setInterval(updatePosition, 30000);
}

function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        zoom: 9,
        center: { 'lat': 42.6, 'lng': -78.1 }
    });

    showDistrictOverlay();
    showRoute();
    showMedia();
    showEvents();
    watchIan();
}