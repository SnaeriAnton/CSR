using UnityEngine;
using Zenject;
using Mirror;

namespace Game
{
    public class SubscriptionNetworkManager : NetworkManager
    {
        private INetworkMessageSubscriptionService _subscriptionService;

        [Inject]
        public void Construct(INetworkMessageSubscriptionService subscriptionService) => _subscriptionService = subscriptionService;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _subscriptionService.InitializeServer();
            Debug.Log("[SubscriptionNetworkManager] Server started.");
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            _subscriptionService.InitializeClient();
            Debug.Log("[SubscriptionNetworkManager] Client started.");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient connection)
        {
            _subscriptionService.RemoveClient(connection);
            Debug.Log($"[SubscriptionNetworkManager] Client disconnected. ConnectionId: {connection.connectionId}");

            base.OnServerDisconnect(connection);
        }

        public override void OnStopServer()
        {
            _subscriptionService.Clear();
            Debug.Log("[SubscriptionNetworkManager] Server stopped. Subscriptions cleared.");

            base.OnStopServer();
        }

        public override void OnStopClient()
        {
            Debug.Log("[SubscriptionNetworkManager] Client stopped.");
            base.OnStopClient();
        }
    }
}
