using System.Collections.Generic;
using Mirror;

namespace Game
{
    public interface INetworkMessageSubscriptionRegistry
    {
        public void AddSubscription(NetworkConnectionToClient connection, string messageTypeId);
        public bool HasSubscription(NetworkConnectionToClient connection, string messageTypeId);
        public IReadOnlyList<NetworkConnectionToClient> GetSubscribers(string messageTypeId);
        public void RemoveClient(NetworkConnectionToClient connection);
        public void Clear();
    }
}
