<template>
    <div>
        <div>
            <div class="red-sircle"></div>
            <span>{{clients.length}}</span>
        </div>
        <div class="container">
            <p>Clients:</p>
            <table>
                <tr>
                    <th>Name</th>
                    <th>Live</th>
                    <th>Send</th>
                    <th>Got</th>
                </tr>
                <tr v-for="client in clients">
                    <td>{{client.Name}}</td>
                    <td>{{client.LiveTime.substr(0,8)}}</td>
                    <td>{{client.messagesSend}}</td>
                    <td>{{client.clientsMessagesGot}}</td>
                </tr>
            </table>
        </div>
        <div></div>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';

    @Component
    export default class ClientsInfo extends Vue {
        private clients: Array<any> = [];
        private chat: any;

        constructor() {
            super();
            let chat = this.chat = window.connection;

            this.chat.client.getClientsInfo = this.getClientsInfo;

            window.connectionStartEvent.done(function () {
                setInterval(chat.server.getClientsInfo, 1000);
            });
        }

        getClientsInfo(clients: any) {
            this.clients = clients;
        }
    }
</script>

<style scoped>
    .container {
        background-color: #99CCFF;
        border: thick solid #808080;
        padding: 20px;
        margin: 20px;
    }
    .red-sircle {
        width: 15px;
        height: 15px;
        border-radius: 15px;
        background-color: #ff0000;
        display: inline-block;
        margin-right: 15px;
    }
</style>