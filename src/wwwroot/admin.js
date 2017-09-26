Vue.config.devtools = true;

new Vue({
    el: '#gfc-admin',
    data: {
        direction_service: new google.maps.DirectionsService(),
        geocoding_service: new google.maps.Geocoder(),
        route: [],
        current_section: {},
        media: [],
        current_media: {},
        events: [],
        current_event: {},
        tracking: false,
        action: 'loading'
    },
    methods: {
        displayDate: function (date) {
            return moment(date).format('l');
        },
        displayDateTime: function (date) {
            return moment(date).format('l LT');
        },
        displayMediaType: function (type) {
            switch (type) {
                case 0: return 'Audio';
                case 1: return 'Video';
                case 2: return 'Image';
                case 3: return 'Text';
            }
            return 'Unknown';
        },
        sorted: function (list) {
            return list.sort(function (a, b) {
                return a.date < b.date
                    ? -1
                    : a.date > b.date
                        ? 1
                        : 0;
            });
        },
        getRoute: function () {
            const app = this;
            axios.get('/route')
                .then(function (response) {
                    app.route = app.sorted(response.data);
                });
        },
        addSection: function () {
            this.current_section = { path: '' };
            this.action = 'route-form';
        },
        editSection: function (section) {
            this.current_section = section;
            this.action = 'route-form';
        },
        updatePath: function (url) {
            const app = this;
            app.current_section.path = '(loading...)';
            if (url === undefined) {
                app.current_section.path = 'Error: URL not defined';
                return;
            }
            const data_part = url.indexOf('data=');
            if (data_part >= 0) {
                const data_strings = url.substring(data_part).split('m2!1d');
                const coord_strings = data_strings.splice(1, data_strings.length - 1);
                const coords = [];
                coord_strings.forEach(function (coordString) {
                    const coord_string_split = coordString.split('!');
                    const lat = parseFloat(coord_string_split[1].substring(2));
                    const lng = parseFloat(coord_string_split[0]);
                    coords.push({ lat: lat, lng: lng });
                });

                const waypoint_coords = coords.splice(1, coords.length - 2);
                const waypoints = [];
                waypoint_coords.forEach(function (waypoint) {
                    waypoints.push({ location: waypoint, stopover: false });
                });
                const data = {
                    origin: coords[0],
                    destination: coords[coords.length - 1],
                    waypoints: waypoints,
                    optimizeWaypoints: true,
                    travelMode: 'WALKING'
                };
                app.direction_service.route(data,
                    function (response, status) {
                        if (status === 'OK') {
                            const overview_path = response.routes[0].overview_path;
                            const path = [];
                            overview_path.forEach(function (point) {
                                path.push({ lat: point.lat(), lng: point.lng() });
                            })
                            app.current_section.path = JSON.stringify(path);
                        }
                    });
            } else {
                app.current_section.path = 'Error: URL has no data';
            }
        },
        submitSection: function () {
            const app = this;
            app.action = 'loading';
            axios.post('/route', app.current_section)
                .then(function (response) {
                    app.route = app.sorted(response.data);
                    app.action = 'route-list';
                });
        },
        updateRouteCache: function () {
            const app = this;
            app.action = 'loading';
            axios.get('/route/cache')
                .then(function (response) {
                    app.route = app.sorted(response.data);
                    app.action = 'route-list';
                });
        },
        deleteSection: function (section) {
            if (confirm('Are you sure you want to delete this section?')) {
                const app = this;
                app.action = 'loading';
                axios.delete(`/route/delete/${section.id}`)
                    .then(function (response) {
                        app.route = app.sorted(response.data);
                        app.action = 'route-list';
                    });
            }
        },

        getMedia: function () {
            const app = this;
            axios.get('/media')
                .then(function (response) {
                    app.media = app.sorted(response.data);
                });
        },
        updateMediaCache: function () {
            const app = this;
            app.action = 'loading';
            axios.get('/media/cache')
                .then(function (response) {
                    app.media = app.sorted(response.data);
                    app.action = 'media-list';

                });
        },
        addMedia: function () {
            this.current_media = { location: '' };
            this.action = 'media-form';
        },
        editMedia: function (media) {
            this.current_media = media;
            this.action = 'media-form';
        },
        getMediaCoords: function (address) {
            const app = this;
            app.current_media.location = '(loading...)';
            app.geocoding_service.geocode({
                address: address
            },
                function (results, status) {
                    if (status === 'OK') {
                        app.current_media.location = `{ lat: ${results[0].geometry.location.lat()}, lng: ${results[0].geometry.location.lng()} }`;
                    }
                }
            );
        },
        submitMedia: function () {
            const app = this;
            app.action = 'loading';
            axios.post('/media', app.current_media)
                .then(function (response) {
                    app.media = app.sorted(response.data);
                    app.action = 'media-list';
                });
        },
        deleteMedia: function (media) {
            if (confirm('Are you sure you want to delete this media?')) {
                const app = this;
                app.action = 'loading';
                axios.delete(`/media/delete/${media.id}`)
                    .then(function (response) {
                        app.media = app.sorted(response.data);
                        app.action = 'media-list';
                    });
            }
        },

        getEvents: function () {
            const app = this;
            axios.get('/events')
                .then(function (response) {
                    app.events = app.sorted(response.data);
                });
        },
        updateEventsCache: function () {
            const app = this;
            app.action = 'loading';
            axios.get('/events/cache')
                .then(function (response) {
                    app.events = app.sorted(response.data);
                    app.action = 'events-list';

                });
        },
        addEvent: function () {
            this.current_event = { location: '' };
            this.action = 'event-form';
        },
        editEvent: function (event_info) {
            this.current_event = event_info;
            this.action = 'event-form';
        },
        getEventCoords: function (address) {
            const app = this;
            app.current_event.location = '(loading...)';
            app.geocoding_service.geocode({
                address: address
            },
                function (results, status) {
                    if (status === 'OK') {
                        app.current_event.location = `{ lat: ${results[0].geometry.location.lat()}, lng: ${results[0].geometry.location.lng()} }`;
                    }
                }
            );
        },
        submitEvent: function () {
            const app = this;
            app.action = 'loading';
            axios.post('/events', app.current_event)
                .then(function (response) {
                    app.events = app.sorted(response.data);
                    app.action = 'events-list';
                });
        },
        deleteEvent: function (event_info) {
            if (confirm('Are you sure you want to delete this event?')) {
                const app = this;
                app.action = 'loading';
                axios.delete(`/events/delete/${event_info.id}`)
                    .then(function (response) {
                        app.events = app.sorted(response.data);
                        app.action = 'events-list';
                    });
            }
        },

        getTracking: function () {
            const app = this;
            axios.get('/location/status')
                .then(function (response) {
                    app.tracking = response.data;
                });
        },
        toggleTracking: function() {
            const app = this;
            axios.post(`/location/${app.tracking ? 'stop' : 'start'}tracking`)
                .then(function (response) {
                    app.tracking = response.data;
                });
        }
    },
    mounted: function () {
        this.getRoute();
        this.getMedia();
        this.getEvents();
        this.getTracking();
        this.action = 'menu';
    }
});