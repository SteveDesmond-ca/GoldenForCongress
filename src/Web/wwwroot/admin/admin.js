Vue.config.devtools = true;

new Vue({
    el: "#gfc-admin",
    data: {
        route: [],
        current_section: {},
        direction_service: new google.maps.DirectionsService(),
        media: [],
        current_media: {},
        action: 'loading'

    },
    methods: {
        displayDate: function (date) {
            return moment(date).format('l');
        },
        displayMediaType: function (type) {
            switch (type) {
                case 0: return 'Audio';
                case 1: return 'Video';
                case 2: return 'Image';
                case 3: return 'Text';
            }
        },
        sortedRoute: function (route) {
            return route.sort(function (a, b) {
                return a.day < b.day
                    ? -1
                    : a.day > b.day
                        ? 1
                        : a.date < b.date
                            ? -1
                            : 1;
            });
        },
        sortedMedia: function (media) {
            return media.sort(function (a, b) {
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
                    app.route = app.sortedRoute(response.data);
                });
        },
        addSection: function () {
            this.current_section = {};
            this.action = 'route-form';
        },
        editSection: function (section) {
            this.current_section = section;
            this.action = 'route-form';
        },
        updatePath: function (url) {
            const app = this;
            app.current_section.path = '(loading...)';
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
                        if (status == "OK") {
                            const overview_path = response.routes[0].overview_path;
                            const path = [];
                            overview_path.forEach(function(point) {
                                path.push({ lat: point.lat(), lng: point.lng() });
                            })
                            app.current_section.path = JSON.stringify(path);
                        }
                    });
            } else {
                axios.get('/route/from-gmaps/' + encodeURIComponent(url))
                    .then(function (response) {
                        app.current_section.path = JSON.stringify(response.data);
                    });
            }
        },
        submitSection: function () {
            const app = this;
            app.action = 'loading';
            axios.post('/route', app.current_section)
                .then(function (response) {
                    app.route = app.sortedRoute(response.data);
                    app.action = 'route-list';
                });
        },
        updateRouteCache: function () {
            const app = this;
            app.action = 'loading';
            axios.get('/route/cache')
                .then(function (response) {
                    app.route = app.sortedRoute(response.data);
                    app.action = 'route-list';
                });
        },
        deleteSection: function (section) {
            if (confirm('Are you sure you want to delete this section?')) {
                const app = this;
                app.action = 'loading';
                axios.delete('/route/' + section.id)
                    .then(function (response) {
                        app.route = app.sortedRoute(response.data);
                        app.action = 'route-list';
                    });
            }
        },

        getMedia: function () {
            const app = this;
            axios.get('/media')
                .then(function (response) {
                    app.media = app.sortedMedia(response.data);
                });
        },
        updateMediaCache: function () {
            const app = this;
            app.action = 'loading';
            axios.get('/media/cache')
                .then(function (response) {
                    app.media = app.sortedMedia(response.data);
                    app.action = 'media-list';

                });
        },
        addMedia: function () {
            this.current_media = {};
            this.action = 'media-form';
        },
        editMedia: function (media) {
            this.current_media = media;
            this.action = 'media-form';
        },
        submitMedia: function () {
            const app = this;
            app.action = 'loading';
            axios.post('/media', app.current_media)
                .then(function (response) {
                    app.media = app.sortedMedia(response.data);
                    app.action = 'media-list';
                });
        },
        deleteMedia: function (media) {
            if (confirm('Are you sure you want to delete this media?')) {
                const app = this;
                app.action = 'loading';
                axios.delete('/delete/' + media.id)
                    .then(function (response) {
                        app.media = app.sortedMedia(response.data);
                        app.action = 'media-list';
                    });
            }
        }
    },
    mounted: function () {
        this.getRoute();
        this.getMedia();
        this.action = 'menu';
    }
});