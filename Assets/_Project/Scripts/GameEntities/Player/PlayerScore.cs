using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using VContainer;
[RequireComponent(typeof(PlayerIdentifiers))]
public class PlayerScore : NetworkBehaviour
{
    [Inject] PlayerNamesUI playerNamesUI;
    [Inject] GameResults gameResults;
    public readonly NetworkVariable<int> Score = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public readonly NetworkVariable<Vector3> LastCheckpoint = new NetworkVariable<Vector3>(Vector3.zero,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool gameEnded;

    private PlayerIdentifiers playerIdentifiers;
    public override void OnNetworkSpawn()
    {
        Score.OnValueChanged += UpdateUI;
        playerIdentifiers = GetComponent<PlayerIdentifiers>();
    }

    public override void OnNetworkDespawn()
    {
        Score.OnValueChanged -= UpdateUI;
    }

    private void UpdateUI(int _, int currentScore)
    {
        playerNamesUI.UpdatePlayerScore(playerIdentifiers.sessionPlayerId.Value.ToString(), currentScore);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void AddScoreServerRpc()
    {
        if (gameEnded) return; 
        Score.Value ++;
        if (Score.Value >= 10)
        {
            string winnerName = playerIdentifiers.sessionPlayerId.Value.ToString();
            gameEnded = true;
            ShowVictoryClientRpc(winnerName);
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void ShowVictoryClientRpc(string winnerName)
    {
      gameResults.ShowResults(winnerName);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void UpdateCheckpointServerRpc(Vector3 position)
    {
        LastCheckpoint.Value = position;
    }
}
