
using Mirror;
using UnityEngine;
using Zenject;

namespace Game
{
    public class HelloMessageDemoScenario : MonoBehaviour
    {
        [SerializeField] private bool _autoSubscribe = true;

        private INetworkMessageSubscriptionService _subscriptionService;
        private bool _subscribed;

        [Inject]
        public void Construct(INetworkMessageSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
            _subscriptionService.ClientSubscribed += OnClientSubscribed;

            Debug.Log("[HelloMessageDemoScenario] Constructed and subscribed to ClientSubscribed event.");
        }

        private void OnDestroy()
        {
            if (_subscriptionService != null)
                _subscriptionService.ClientSubscribed -= OnClientSubscribed;
        }

        private void Update()
        {
            if (_autoSubscribe == false) return;
            if (_subscribed) return;
            if (NetworkClient.isConnected == false) return;

            SubscribeAsClient();

            _subscribed = true;
        }

        private void SubscribeAsClient()
        {
            if (NetworkClient.isConnected == false)
            {
                Debug.LogWarning("[HelloMessageDemoScenario] Client is not connected.");
                return;
            }

            _subscriptionService.Subscribe<HelloMessage>((message) => Debug.Log($"[HelloMessageDemoScenario] Received message: {message.Text}"));
        }

        private void OnClientSubscribed(NetworkConnectionToClient connection, string messageTypeId)
        {
            Debug.Log($"[HelloMessageDemoScenario] Server detected subscription: {messageTypeId}");

            if (NetworkServer.active == false)
                return;

            if (messageTypeId != NetworkMessageTypeId.Get<HelloMessage>())
                return;

            _subscriptionService.SendToSubscriber(connection, new HelloMessage
            {
                Text = "Hello Client!"
            });
        }
    }
}