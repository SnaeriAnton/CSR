using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Game
{
    public class NetworkMessageSubscriptionService : INetworkMessageSubscriptionService
    {
        private readonly INetworkMessageSubscriptionRegistry _registry;

        private bool _serverInitialized;
        private bool _clientInitialized;

        public event Action<NetworkConnectionToClient, string> ClientSubscribed;

        public NetworkMessageSubscriptionService(INetworkMessageSubscriptionRegistry registry) => _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        public void RemoveClient(NetworkConnectionToClient connection) => _registry.RemoveClient(connection);
        public void Clear() => _registry.Clear();
        
        public void InitializeServer()
        {
            if (_serverInitialized)
                return;

            NetworkServer.RegisterHandler<SubscribeNetworkMessage>(OnClientSubscribed);

            _serverInitialized = true;

            Debug.Log("[NetworkMessageSubscriptionService] Server initialized.");
        }

        public void InitializeClient()
        {
            if (_clientInitialized)
                return;

            _clientInitialized = true;

            Debug.Log("[NetworkMessageSubscriptionService] Client initialized.");
        }

        public void Subscribe<TMessage>(Action<TMessage> handler) where TMessage : struct, NetworkMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            string messageTypeId = NetworkMessageTypeId.Get<TMessage>();

            NetworkClient.RegisterHandler(handler);

            NetworkClient.Send(new SubscribeNetworkMessage
            {
                MessageTypeId = messageTypeId
            });

            Debug.Log($"[NetworkMessageSubscriptionService] Client subscribed to {messageTypeId}.");
        }

        public void SendToSubscriber<TMessage>(NetworkConnectionToClient connection, TMessage message) where TMessage : struct, NetworkMessage
        {
            if (connection == null)
            {
                Debug.LogWarning("[NetworkMessageSubscriptionService] Connection is null.");
                return;
            }

            string messageTypeId = NetworkMessageTypeId.Get<TMessage>();

            if (_registry.HasSubscription(connection, messageTypeId) == false)
            {
                Debug.LogWarning($"[NetworkMessageSubscriptionService] Client {connection.connectionId} is not subscribed to {messageTypeId}.");
                return;
            }

            connection.Send(message);

            Debug.Log($"[NetworkMessageSubscriptionService] Sent {messageTypeId} to client {connection.connectionId}.");
        }

        private void OnClientSubscribed(NetworkConnectionToClient connection, SubscribeNetworkMessage message)
        {
            if (connection == null)
                return;

            if (string.IsNullOrWhiteSpace(message.MessageTypeId))
                return;

            _registry.AddSubscription(connection, message.MessageTypeId);

            Debug.Log($"[NetworkMessageSubscriptionService] Client {connection.connectionId} subscribed to {message.MessageTypeId}.");

            ClientSubscribed?.Invoke(connection, message.MessageTypeId);
        }
    }
}