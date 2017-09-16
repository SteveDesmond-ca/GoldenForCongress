Vue.config.devtools = true;

new Vue({
    el: "#gfc-admin",
    data: {
        route: [],
        current_section: {},
        media: [],
        current_media: {},
        current_location: {},
        watch_id: null,
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
            const vthis = this;
            axios.get('/route')
                .then(function (response) {
                    vthis.route = vthis.sortedRoute(response.data);
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
        submitSection: function () {
            this.action = 'loading';
            const vthis = this;
            axios.post('/route', vthis.current_section)
                .then(function (response) {
                    vthis.route = vthis.sortedRoute(response.data);
                    vthis.action = 'route-list';
                });
        },
        updateRouteCache: function () {
            this.action = 'loading';
            const vthis = this;
            axios.get('/route/cache')
                .then(function (response) {
                    vthis.route = vthis.sortedRoute(response.data);
                    vthis.action = 'route-list';
                });
        },
        deleteSection: function (section) {
            if (confirm('Are you sure you want to delete this section?')) {
                this.action = 'loading';
                const vthis = this;
                axios.delete('/route/' + section.id)
                    .then(function (response) {
                        vthis.route = vthis.sortedRoute(response.data);
                        vthis.action = 'route-list';
                    });
            }
        },

        getMedia: function () {
            const vthis = this;
            axios.get('/media')
                .then(function (response) {
                    vthis.media = vthis.sortedMedia(response.data);
                });
        },
        updateMediaCache: function () {
            this.action = 'loading';
            const vthis = this;
            axios.get('/media/cache')
                .then(function (response) {
                    vthis.media = vthis.sortedMedia(response.data);
                    vthis.action = 'media-list';

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
            this.action = 'loading';
            const vthis = this;
            axios.post('/media', vthis.current_media)
                .then(function (response) {
                    vthis.media = vthis.sortedMedia(response.data);
                    vthis.action = 'media-list';
                });
        },
        deleteMedia: function (media) {
            if (confirm('Are you sure you want to delete this media?')) {
                this.action = 'loading';
                const vthis = this;
                axios.delete('/delete/' + media.id)
                    .then(function (response) {
                        vthis.media = vthis.sortedMedia(response.data);
                        vthis.action = 'media-list';
                    });
            }
        },

        getLocation: function () {
            const vthis = this;
            axios.get('/ian.json')
                .then(function (response) {
                    vthis.current_location = response.data;
                });
        },
        startTracking: function () {
            this.watch_id = navigator.geolocation.watchPosition(function (position) {
                this.current_location = {
                    position: { lat: position.coords.latitude, lng: position.coords.longitude },
                    time: moment(position.timestamp).format()
                };
                const vthis = this;
                const location_to_send = this.current_location;
                location_to_send.position = JSON.stringify(location_to_send.position);
                axios.post('/location', location_to_send)
                    .then(function (response) {
                        vthis.media = vthis.sortedMedia(response.data);
                        vthis.action = 'media-list';
                    });
            },
                function (error) {
                    console.log(error);
                },
                {
                    enableHighAccuracy: true,
                    timeout: 5000,
                    maximumAge: 0
                }
            );
        },
        stopTracking: function () {
            navigator.geolocation.clearWatch(this.watch_id);
            this.watch_id = null;
        }
    },
    mounted: function () {
        this.getRoute();
        this.getMedia();
        this.getLocation();
        this.action = 'menu';
    }
});