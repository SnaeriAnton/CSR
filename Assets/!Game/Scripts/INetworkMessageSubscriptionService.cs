using System;
using Mirror;

namespace Game
{
    public interface INetworkMessageSubscriptionService
    {
        public event Action<NetworkConnectionToClient, string> ClientSubscribed;
        
        public void InitializeServer();
        public void InitializeClient();
        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : struct, NetworkMessage;
        public void SendToSubscriber<TMessage>(NetworkConnectionToClient connection, TMessage message) where TMessage : struct, NetworkMessage;
        public void RemoveClient(NetworkConnectionToClient connection);
        public void Clear();
    }
}