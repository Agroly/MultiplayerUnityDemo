using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class VContainerPrefabHandler : INetworkPrefabInstanceHandler
{
    private readonly IObjectResolver _resolver;
    private readonly GameObject _prefab;

    public VContainerPrefabHandler(IObjectResolver resolver, GameObject prefab)
    {
        _resolver = resolver;
        _prefab = prefab;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    { 
        var instance = _resolver.Instantiate(_prefab, position, rotation);

        return instance.GetComponent<NetworkObject>();
    }

    public void Destroy(NetworkObject networkObject)
    {
        Object.Destroy(networkObject.gameObject);
    }
}