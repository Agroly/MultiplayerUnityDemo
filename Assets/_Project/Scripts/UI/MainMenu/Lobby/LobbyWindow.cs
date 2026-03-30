using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VContainer;
using Assets._Project.Scripts.UI.Tools;
using Unity.VisualScripting;

public class LobbyWindow : MonoBehaviour
{
    [Inject] private SessionManager sessionManager;
    [Inject] private CanvasManager canvasManager;
    [SerializeField] private TMP_InputField Code;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private PlayerVisualModel playerEntryPrefab;
    [SerializeField] private Button LeaveButton;
    [SerializeField] private Button StartButton;

    private Dictionary<string, PlayerVisualModel> currentPlayers = new Dictionary<string, PlayerVisualModel>();
    private void Start()
    {
        sessionManager.PlayerJoined += AddPlayerToUI;
        sessionManager.PlayerLeft += RemovePlayerFromUI;
        sessionManager.SessionCreated += Show;
        sessionManager.PlayersReady += ActivateStartButton;
        sessionManager.CheckSession();
        StartButton.onClick.AddListener(OnStartButtonClicked);
        LeaveButton.onClick.AddListener(OnLeaveButtonClicked);
    }
    private void Show(Dictionary<string,string> players, string joinCode)
    {
        ClearUI();
        foreach (var player in players)
        {
            AddPlayerToUI(player.Key, player.Value);
        }
        Code.text = joinCode;
        StartButton.gameObject.SetActive(sessionManager.session.IsHost);
        StartButton.interactable = false;
        canvasManager.SwitchCanvas(GetComponent<SwitchableCanvasGroup>());
    }
    
    private void OnDestroy()
    {
        if (sessionManager != null)
        {
            sessionManager.PlayerJoined -= AddPlayerToUI;
            sessionManager.PlayerLeft -= RemovePlayerFromUI;
            sessionManager.PlayersReady -= ActivateStartButton;
        }
        sessionManager.SessionCreated -= Show;
        LeaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
        StartButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void AddPlayerToUI(string playerId, string playerName)
    {
        var entry = Instantiate(playerEntryPrefab, playerListParent);
        entry.SetName(playerName);
        currentPlayers.Add(playerId, entry);
    }

    private void RemovePlayerFromUI(string playerId, string playerName)
    {
        currentPlayers.TryGetValue(playerId, out var player);
        Destroy(player.gameObject);
        currentPlayers.Remove(playerId);
        if (currentPlayers.Count < sessionManager.session.MaxPlayers) StartButton.interactable = false;
    }
    private void ClearUI()
    {
        foreach (var player in currentPlayers.Values)
        {
            Destroy(player.gameObject);
        }
        currentPlayers.Clear();
    }

    private void ActivateStartButton()
    {
        StartButton.interactable = true;
    }

    private async void OnLeaveButtonClicked()
    {
        await sessionManager.LeaveSession();
    }

    private void OnStartButtonClicked()
    {
        sessionManager.LoadScene("Game");
    }
}