const snap_to_roads = true;

function initRouteMap() {
	map = new google.maps.Map(document.getElementById('route-map'),
	{
		zoom: 9,
		center: { "lat": 42.6, "lng": -78.1 }
	});

	var directionsService = new google.maps.DirectionsService();
	var drawingManager = new google.maps.drawing.DrawingManager({
		map: map,
		drawingMode: google.maps.drawing.OverlayType.POLYLINE,
		drawingControl: false
	});
	google.maps.event.addListener(drawingManager, 'polylinecomplete', function (line) {
		if (snap_to_roads) {
			var path = [];
			var waypoints = [];
			var linePath = line.getPath();
			for (n = 1; n < linePath.getLength() - 1; n++) {
				waypoints.push({
					"location": linePath.getAt(n)
				});
			}
			directionsService.route({
				origin: linePath.getAt(0),
				destination: linePath.getAt(n),
				waypoints: waypoints,
				travelMode: google.maps.DirectionsTravelMode.WALKING
			}, function (result, status) {
				if (status == google.maps.DirectionsStatus.OK) {
					for (var i = 0, len = result.routes[0].overview_path.length; i < len; i++) {
						path.push(result.routes[0].overview_path[i]);
					}
				}
				document.getElementById('path').value = JSON.stringify(path);
				line.setPath(path);
			});
		} else {
		    document.getElementById('path').value = JSON.stringify(line.getPath().getArray());
		}
	});
    //toggleRouteMap();
}

function toggleRouteMap() {
    var e = document.getElementById('route-map-wrapper');
    if (e.className == 'd-none')
        e.className = '';
    else
        e.className = 'd-none';
}