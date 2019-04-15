<template>
    <div id="app">
        <table>
            <tr>
                <td><Chat /></td>
                <td style="vertical-align: top;"><ClientsInfo /></td>
            </tr>
        </table>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Chat from './components/Chat.vue';
    import ClientsInfo from './components/ClientsInfo.vue';
    var $ = window.jQuery = require('jquery');
    require('signalr');

    @Component({
        components: {
            Chat, ClientsInfo
        }
    })

    export default class App extends Vue {
        constructor() {
            super();
            this.initConnection();
        }

        initConnection() {
            let chat = window.connection;
            window.name = prompt('Enter your name', '') as string;

            chat.client.stopClient = this.stopClient;

            window.connectionStartEvent.done(function () {
                chat.server.register(window.name);
            })
            .fail(function () {
                alert("fail");
            });
        }

        stopClient() {
            $.connection.hub.stop();
        }
    }
</script>

<style>
</style>
