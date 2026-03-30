using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class GameScope : LifetimeScope
{
    [SerializeField] private GameObject _planePrefab;
    [SerializeField] private PlayerNamesUI _playerNamesUI;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<PlayerInput>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.RegisterEntryPoint<NetworkPrefabRegistrationHandler>()
               .WithParameter(_planePrefab);
        builder.RegisterInstance(_playerNamesUI);
        builder.RegisterComponentInHierarchy<CountDownController>();
        builder.RegisterComponentInHierarchy<PlayerSpawner>();
        builder.RegisterComponentInHierarchy<GameResults>();
        builder.RegisterBuildCallback(resolver =>
        {
            resolver.Inject(_playerNamesUI);
        });
        
    }
}
