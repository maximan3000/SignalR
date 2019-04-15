import Vue from 'vue';
import App from './App.vue';
import LoadScript from 'vue-plugin-load-script';
Vue.use(LoadScript);
var $ = window.jQuery = require('jquery');
require('signalr');

Vue.config.productionTip = true;

declare global {
    interface Window {
        jQuery: Function;
        connection: any;
        connectionStartEvent: any;
        name: string;
    }
}

declare module 'vue/types/vue' {
    interface VueConstructor {
        loadScript: Function;
    }
}

Vue.loadScript('http://localhost:53311/signalr/hubs')
    .then(() => {
        window.connection = $.connection.groupChatHub;

        $.connection.hub.url = 'http://localhost:53311/signalr';
        window.connectionStartEvent = $.connection.hub.start();
            
        new Vue({
            render: h => h(App)
        }).$mount('#app');
    })
    .catch(() => {
        alert('Connection failed!');
    });
