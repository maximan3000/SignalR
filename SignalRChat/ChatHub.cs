using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    public abstract class ChatHub : Hub
    {
        public const string ServerName = "Server";

        protected enum AlertType {
            Connected,
            Disconnected
        }

        protected enum AlertTypeSingle
        {
            Whisper,
            Greeting
        }

        protected static ConcurrentDictionary<string, ClientInfo> 
            clientsInfo = new ConcurrentDictionary<string, ClientInfo>();
        
        public void getClientsInfo() {
            //TODO implements getClientsInfo callback on client's side
            Clients.Client(Context.ConnectionId).getClientsInfo(clientsInfo.Values);
        }

        public void getMyInfo()
        {
            //TODO implements getMyInfo callback on client's side
            Clients.Client(Context.ConnectionId).getMyInfo(clientsInfo[Context.ConnectionId]);
        }

        public void Register(string name)
        {
            //TODO if (name is bad - like not unique or empty) => restrict access
            name = SecureName(name);
            ClientInfo clientInfo = new ClientInfo(name, DateTime.Now);
            clientsInfo.GetOrAdd(Context.ConnectionId, clientInfo);

            Alert(AlertType.Connected, Context.ConnectionId);
        }

        public void Broadcast(string message)
        {
            ClientInfo clientInfo;
            clientsInfo.TryGetValue(Context.ConnectionId, out clientInfo);

            //TODO implements broadcastMessage callback on client's side
            Interlocked.Increment(ref clientInfo.messagesSend);
            foreach (var entry in clientsInfo)
            {
                if (entry.Key != Context.ConnectionId)
                    Interlocked.Increment(ref entry.Value.clientsMessagesGot);
            }
            Clients.All.broadcastMessage(clientInfo.Name, message);
        }

        public void Whisper(string targetName, string message)
        {
            targetName = SecureName(targetName);
            KeyValuePair<string, ClientInfo> targetClient = 
                clientsInfo.FirstOrDefault(entry => entry.Value.Name == targetName);

            if (string.IsNullOrEmpty(targetClient.Key))
            {
                AlertSingle(AlertTypeSingle.Whisper, Context.ConnectionId);
                return;
            }

            ClientInfo clientInfo;
            clientsInfo.TryGetValue(Context.ConnectionId, out clientInfo);

            //TODO implements whisper callback on client's side
            Interlocked.Increment(ref clientInfo.messagesSend);
            Interlocked.Increment(ref targetClient.Value.clientsMessagesGot);
            Clients.Clients(new List<string>() { targetClient.Key, Context.ConnectionId }).whisper(clientInfo.Name, message);
        }

        public override Task OnConnected()
        {
            AlertSingle(AlertTypeSingle.Greeting, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Alert(AlertType.Disconnected, Context.ConnectionId);

            ClientInfo clientInfo;
            clientsInfo.TryRemove(Context.ConnectionId, out clientInfo);
            return base.OnDisconnected(stopCalled);
        }

        protected string SecureName(string name)
        {
            return name.Trim();
        }

        protected void AlertSingle(AlertTypeSingle alertType, string connectionId)
        {
            string message = "";

            switch (alertType)
            {
                case AlertTypeSingle.Whisper:
                    message = "No such username!";
                    break;
                case AlertTypeSingle.Greeting:
                    message = "Welcome to the server!";
                    break;
                default:
                    break;
            }

            Clients.Client(connectionId).whisper(ServerName, message);
        }

        protected void Alert(AlertType alertType, string connectionId)
        {
            ClientInfo clientInfo;
            clientsInfo.TryGetValue(connectionId, out clientInfo);
            string message = "";

            switch (alertType)
            {
                case AlertType.Connected:
                    message = $"Client {clientInfo.Name} connected!";
                    break;
                case AlertType.Disconnected:
                    message = $"Client {clientInfo.Name} left!";
                    break;
                default:
                    break;
            }

            Clients.All.broadcastMessage(ServerName, message);
        }
    }
}