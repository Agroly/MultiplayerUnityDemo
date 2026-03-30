using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class NetworkPrefabRegistrationHandler : IStartable, IDisposable
{
    private readonly IObjectResolver _resolver;
    private readonly GameObject _planePrefab;

    public NetworkPrefabRegistrationHandler(IObjectResolver resolver, GameObject planePrefab)
    {
        _resolver = resolver;
        _planePrefab = planePrefab;
    }

    // Вызовется VContainer-ом сразу после создания контейнера, но до OnNetworkSpawn
    public void Start()
    {
        if (NetworkManager.Singleton == null) return;

        var handler = new VContainerPrefabHandler(_resolver, _planePrefab);
        NetworkManager.Singleton.PrefabHandler.AddHandler(_planePrefab, handler);

        Debug.Log($"[Network] Prefab {_planePrefab.name} registered via EntryPoint");
    }

    public void Dispose()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.PrefabHandler != null)
        {
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(_planePrefab);
        }
    }
}