using Mirror;
using UnityEngine;
using Zenject;

namespace Game
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private SimpleNetworkManagerHUD _hud;
        [SerializeField] private NetworkManager _networkManager;

        public override void InstallBindings()
        {
            Container
                .Bind<INetworkMessageSubscriptionRegistry>()
                .To<NetworkMessageSubscriptionRegistry>()
                .AsSingle();

            Container
                .Bind<INetworkMessageSubscriptionService>()
                .To<NetworkMessageSubscriptionService>()
                .AsSingle();

            Container
                .BindInterfacesTo<SimpleNetworkManagerHUD>()
                .FromInstance(_hud)
                .AsSingle();

            Container
                .Bind<NetworkManager>()
                .FromInstance(_networkManager)
                .AsSingle();
        }
    }
}