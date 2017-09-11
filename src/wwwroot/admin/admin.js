new Vue({
    el: "#gfc-admin",
    data: {
        routes: []
    },
    methods: {
        displayDate: function (date) {
            return moment(date).calendar();
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
        updateRoute: function () {
            var vthis = this;
            axios.get('/route')
                .then(function (response) {
                    vthis.routes = vthis.sortedRoute(response.data);
                });
        },
        updateRouteCache: function () {
            var vthis = this;
            axios.get('/route/cache')
                .then(function (response) {
                    vthis.routes = vthis.sortedRoute(response.data);
                    alert('Done!');
                });
        },
        deleteSection: function (route) {
            if (confirm('Are you sure you want to delete this section?')) {
                var vthis = this;
                axios.delete('/route/' + route.id)
                    .then(function (response) {
                        vthis.routes = vthis.sortedRoute(response.data);
                    });
            }
        }
    },
    mounted: function () {
        this.updateRoute();
    }
});