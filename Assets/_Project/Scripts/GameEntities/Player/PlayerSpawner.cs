using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private PlayerMovement _planePrefab;
    [SerializeField] private Transform[] _spawnPoints;

    private IObjectResolver _resolver;
    private GameManager gameManager;
    [Inject]
    public void Construct(IObjectResolver resolver, GameManager gameManager)
    {
        _resolver = resolver;
        this.gameManager = gameManager;
        this.gameManager.SetState(GameState.Loading);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null && NetworkManager.SceneManager != null)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
        }
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "Game" && NetworkManager.IsServer)
        {
            if (clientsCompleted.Count == NetworkManager.ConnectedClientsList.Count)
            {
                Debug.Log("All clients loaded the scene: " + sceneName);
                int i = 0;
                foreach(var client in clientsCompleted)
                {
                    i++;
                    SpawnPlayer(client, _spawnPoints[i]);
                }
                SetGameStateGameClientRpc();
            }
        }
    }

    private void SpawnPlayer(ulong clientId, Transform spawnpoint)
    {
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
        {
            Debug.LogWarning($"У игрока {clientId} уже есть PlayerObject. Пропускаем ручной спавн.");
            return;
        }

        Vector3 pos = spawnpoint.position;
        Quaternion rot = spawnpoint.rotation;

        // Внедряем зависимости через VContainer
        PlayerMovement playerInstance = _resolver.Instantiate(_planePrefab, pos, rot);

        var networkObject = playerInstance.GetComponent<NetworkObject>();

        // Спавним как основной объект игрока
        networkObject.SpawnAsPlayerObject(clientId);

        Debug.Log($"[Server] Заспавнен самолет для клиента {clientId} в точке {spawnpoint}");
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetGameStateGameClientRpc()
    {
        // Этот код выполнится НА ВСЕХ клиентах (и на хосте тоже)
        Debug.Log("ClientRpc: Переключаем состояние в Game");
        gameManager.SetState(GameState.InGame);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RespawnPlayerServerRpc(RpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;

        if (!NetworkManager.ConnectedClients.TryGetValue(requesterId, out var client)) return;

        var playerObj = client.PlayerObject;
        if (playerObj == null) return;

        var score = playerObj.GetComponent<PlayerScore>();
        Vector3 targetPos = score.LastCheckpoint.Value;

        if (targetPos == Vector3.zero)
        {
            targetPos = _spawnPoints[requesterId % (ulong)_spawnPoints.Length].position;
        }

        var rpcOptions = new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Single(requesterId, RpcTargetUse.Temp)
            }
        };

        RespawnClientRpc(targetPos, rpcOptions);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void RespawnClientRpc(Vector3 targetPos, RpcParams rpcParams = default)
    {
        var myPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (myPlayer != null)
        {
            if (myPlayer.TryGetComponent<Rigidbody>(out var rb))
            {
                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                rb.position = targetPos;
                rb.rotation = Quaternion.identity;
            }
            if (myPlayer.TryGetComponent<NetworkTransform>(out var nt))
            {
                nt.Teleport(targetPos, Quaternion.identity, myPlayer.transform.localScale);
            }
            else
            {
                myPlayer.transform.SetPositionAndRotation(targetPos, Quaternion.identity);
            }

            Debug.Log($"[Client] Мой самолет успешно респавнулся в {targetPos}");
        }
        else
        {
            Debug.LogError("[Client] Не удалось найти локальный PlayerObject для респавна!");
        }
    }

    public void DespawnPlayers()
    {
        if (!IsServer) return;
        foreach (var client in NetworkManager.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.Despawn(true);
            }
        }
    }
}