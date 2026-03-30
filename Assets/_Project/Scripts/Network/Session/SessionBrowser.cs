using Unity.Services.Multiplayer;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using Assets._Project.Scripts.UI.MainMenu.Lobby;
using System.Runtime.Serialization;
using VContainer;
using VContainer.Unity;
using Assets._Project.Scripts.UI.Tools;
using System.Collections.Generic;

public class SessionBrowser : MonoBehaviour
{
    [Inject] private GameManager gameManager;
    [Inject] private IObjectResolver resolver;
    [Inject] private CanvasManager canvasManager;
    [SerializeField] private Transform listParent;
    [SerializeField] private LobbyVisualModel lobbyPrefab;
    [SerializeField] private Button viewLobbyButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private SwitchableCanvasGroup mainMenu;

    private List<LobbyVisualModel> lobbies = new List<LobbyVisualModel>();
    private void Awake()
    {
        viewLobbyButton.onClick.AddListener(LogSessions);
        refreshButton.onClick.AddListener(LogSessions);
        closeButton.onClick.AddListener(Close);
    }
    private async void LogSessions()
    {
        await ListAvailableSessions();
    }
    private async Task ListAvailableSessions()
    {
        Clear();
        try
        {
            var options = new QuerySessionsOptions()
            {
                Count = 10
            };
            gameManager.SetState(GameState.Loading);
            var query = await MultiplayerService.Instance.QuerySessionsAsync(options);
            gameManager.SetState(GameState.Idle);

            foreach (var s in query.Sessions)
            {
                if (s.IsLocked) return;
                    LobbyVisualModel go = resolver.Instantiate(lobbyPrefab, listParent);
                    go.Init(s.Name, (s.MaxPlayers - s.AvailableSlots), s.MaxPlayers, s.Id);
                    lobbies.Add(go);
                Debug.Log(s.Id);
            }
        }
        catch (System.Exception e)
        {
            gameManager.SetState(GameState.Idle);
            Debug.LogError($"Failed to query sessions: {e}");
        }
    }
    private void Clear()
    {
        foreach (var lobby in lobbies)
        {
            if (lobby != null) Destroy(lobby.gameObject);
        }
        lobbies.Clear();
    }
    private void Close()
    {
        canvasManager.SwitchCanvas(mainMenu);
    }
}