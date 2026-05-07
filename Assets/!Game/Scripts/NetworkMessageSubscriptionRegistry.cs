using System;
using System.Collections.Generic;
using Mirror;

namespace Game
{
    public class NetworkMessageSubscriptionRegistry : INetworkMessageSubscriptionRegistry
    {
        private readonly Dictionary<int, NetworkConnectionToClient> _connectionsById = new();
        private readonly Dictionary<int, HashSet<string>> _messageTypesByConnectionId = new();
        private readonly Dictionary<string, HashSet<int>> _connectionIdsByMessageType = new();

        public void AddSubscription(NetworkConnectionToClient connection, string messageTypeId)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (string.IsNullOrWhiteSpace(messageTypeId))
                throw new ArgumentException("Message type id cannot be null or empty.", nameof(messageTypeId));

            int connectionId = connection.connectionId;

            _connectionsById[connectionId] = connection;

            if (_messageTypesByConnectionId.TryGetValue(connectionId, out HashSet<string> messageTypes) == false)
            {
                messageTypes = new HashSet<string>();
                _messageTypesByConnectionId.Add(connectionId, messageTypes);
            }

            messageTypes.Add(messageTypeId);

            if (_connectionIdsByMessageType.TryGetValue(messageTypeId, out HashSet<int> connectionIds) == false)
            {
                connectionIds = new HashSet<int>();
                _connectionIdsByMessageType.Add(messageTypeId, connectionIds);
            }

            connectionIds.Add(connectionId);
        }

        public bool HasSubscription(NetworkConnectionToClient connection, string messageTypeId)
        {
            if (connection == null) return false;

            if (string.IsNullOrWhiteSpace(messageTypeId))
                return false;

            int connectionId = connection.connectionId;

            return _messageTypesByConnectionId.TryGetValue(connectionId, out HashSet<string> messageTypes)
                   && messageTypes.Contains(messageTypeId);
        }

        public IReadOnlyList<NetworkConnectionToClient> GetSubscribers(string messageTypeId)
        {
            if (string.IsNullOrWhiteSpace(messageTypeId))
                return Array.Empty<NetworkConnectionToClient>();

            if (_connectionIdsByMessageType.TryGetValue(messageTypeId, out HashSet<int> connectionIds) == false)
                return Array.Empty<NetworkConnectionToClient>();

            List<NetworkConnectionToClient> subscribers = new();

            foreach (int connectionId in connectionIds)
                if (_connectionsById.TryGetValue(connectionId, out NetworkConnectionToClient connection))
                    subscribers.Add(connection);

            return subscribers;
        }

        public void RemoveClient(NetworkConnectionToClient connection)
        {
            if (connection == null) return;

            int connectionId = connection.connectionId;

            if (_messageTypesByConnectionId.TryGetValue(connectionId, out HashSet<string> messageTypes))
            {
                foreach (string messageTypeId in messageTypes)
                {
                    if (_connectionIdsByMessageType.TryGetValue(messageTypeId, out HashSet<int> connectionIds))
                    {
                        connectionIds.Remove(connectionId);

                        if (connectionIds.Count == 0)
                        {
                            _connectionIdsByMessageType.Remove(messageTypeId);
                        }
                    }
                }

                _messageTypesByConnectionId.Remove(connectionId);
            }

            _connectionsById.Remove(connectionId);
        }

        public void Clear()
        {
            _connectionsById.Clear();
            _messageTypesByConnectionId.Clear();
            _connectionIdsByMessageType.Clear();
        }
    }
}