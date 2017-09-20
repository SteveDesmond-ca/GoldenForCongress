Vue.config.devtools = true;

new Vue({
    el: '#feed-app',
    data: {
        route: [],
        media: [],
        events: []
    },
    methods: {
        getRoute: function() {
            const app = this;
            axios.get('route.json')
                .then(function(response) {
                    app.route = app.sortedRoute(response.data);
                });
        },
        sortedRoute: function(route) {
            return route
                .filter(function (section) {
                    return moment(section.date) > moment();
                })
                .sort(function (a, b) {
                return a.date < b.date
                    ? -1
                    : a.date > b.date
                    ? 1
                    : 0;
            });
        },

        getMedia: function() {
            const app = this;
            axios.get('media.json')
                .then(function(response) {
                    app.media = app.sortedMedia(response.data);
                });
        },
        sortedMedia: function(media) {
            return media.sort(function(a, b) {
                return a.date > b.date
                    ? -1
                    : a.date < b.date
                    ? 1
                    : 0;
            });
        },

        getEvents: function () {
            const app = this;
            axios.get('events.json')
                .then(function(response) {
                    app.events = app.sortedEvents(response.data);
                });
        },
        sortedEvents: function(events) {
            return events
                .filter(function(e) {
                    return moment(e.date) > moment();
                })
                .sort(function (a, b) {
                return a.date > b.date
                    ? -1
                    : a.date < b.date
                    ? 1
                    : 0;
            });
        }
    },
    mounted: function() {
        this.getRoute();
        this.getMedia();
        this.getEvents();
    }
});