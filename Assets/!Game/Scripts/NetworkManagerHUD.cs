using System;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class SimpleNetworkManagerHUD : MonoBehaviour, IDisposable
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private Button _serverButton;
        [SerializeField] private Button _stopButton;

        private NetworkManager _networkManager;

        [Inject]
        public void Construct(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            
            _stopButton.interactable = false;
            
            _hostButton.onClick.AddListener(Host);
            _clientButton.onClick.AddListener(Client);
            _serverButton.onClick.AddListener(Server);
            _stopButton.onClick.AddListener(Stop);
        }

        public void Dispose()
        {
            _hostButton.onClick.RemoveAllListeners();
            _clientButton.onClick.RemoveAllListeners();
            _serverButton.onClick.RemoveAllListeners();
            _stopButton.onClick.RemoveAllListeners();
        }

        private void Host()
        {
            _networkManager.StartHost();
            SetActiveButton(false);
        }

        private void Client() => _networkManager.StartClient();

        private void Server()
        {
            SetActiveButton(false);
            _networkManager.StartServer();
        }

        private void Stop()
        {
            SetActiveButton(true);
            _networkManager.StopHost();
        }

        private void SetActiveButton(bool active)
        {
            _hostButton.interactable = active;
            _clientButton.interactable = active;
            _serverButton.interactable = active;
            _stopButton.interactable = !active;
        }
    }
}