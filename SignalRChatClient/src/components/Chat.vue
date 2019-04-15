<template>
    <div>
        <div class="container">
            <span>Tags: </span>
            <div class="tag-block" v-for="tag in subscriptions" v-if="tag">{{ tag }}</div>
        </div>
        <div class="container">
            <ul class="message-box">
                <li class="message" v-for="message in messages">
                    <strong>{{message.name}}: &nbsp;&nbsp;</strong> {{message.value}}
                </li>
            </ul>
        </div>
        <div class="container">
            <input class="text-box" type="text" v-model="message" />
            <input class="send-button" type="button" value="Send" v-on:click="parseMessage" />
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import Message from '../models/Message';

    @Component
    export default class Chat extends Vue {
        private messages: Array<Message> = [];
        private message: string = "";
        private subscriptions: Array<string> = [];
        private chat: any;

        constructor() {
            super();
            this.chat = window.connection;

            this.chat.client.broadcastMessage = this.broadcastMessage;
            this.chat.client.whisper = this.whisper;
            this.chat.client.gotNews = this.gotNews;
            this.chat.client.getRelevantNews = this.getRelevantNews;
            this.chat.client.groupCast = this.groupCast;
        }

        broadcastMessage(name: string, message: string) {
            let msg = new Message(`(broadcast) ${name}`, message);
            this.messages.push(msg);
        }

        whisper(name: string, message: string) {
            let msg = new Message(`(whisper) ${name}`, message);
            this.messages.push(msg);
        }

        groupCast(name: string, group: string, message: string) {
            let msg = new Message(`(group ${group}) ${name}`, message);
            this.messages.push(msg);
        };

        gotNews(name: string, news: any) {
            let msg = new Message(`${name}`, news.Title);
            this.messages.push(msg);
        }

        getRelevantNews(relevantNews: any) {
            console.dir(relevantNews);
        }

        parseMessage() {
            var message = this.message;
            var params = message.split(" ");
            var command = params[0];

            switch (command) {
                case "/whisper":
                    var name = params[1];
                    delete params[0]; delete params[1];
                    var message = params.join(" ").trim();
                    this.chat.server.whisper(name, message);
                    break;
                case "/subscribe":
                    var tag = params[1];
                    this.chat.server.subscribe(tag);
                    this.subscriptions.push(tag);
                    break;
                case "/unsubscribe":
                    var tag = params[1];
                    this.chat.server.unsubscribe(tag);
                    var index = this.subscriptions.findIndex(x => x == tag);
                    delete this.subscriptions[index];
                    break;
                case "/group":
                    var group = params[1];
                    delete params[0]; delete params[1];
                    var message = params.join(" ").trim();
                    this.chat.server.groupCast(group, message);
                    break;
                default:
                    this.chat.server.broadcast(message);
            }
            this.message = "";
        }
    }
</script>

<style scoped>
    .container {
        background-color: #99CCFF;
        border: thick solid #808080;
        padding: 20px;
        margin: 20px;
        width: 400px;
    }
    .message-box {
        height: 300px;
        overflow-y: scroll;
    }
    .message {
        padding: 5px;
        margin-right: 20px;
    }
    .tag-block {
        padding: 5px;
        background-color: #ffffff;
        display: inline-block;
        margin: 5px;
    }
</style>