using Assets._Project.Scripts.UI.Tools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using static UnityEngine.InputSystem.DefaultInputActions;
using static UnityEngine.Rendering.CoreUtils;

public class SessionManager : IAsyncStartable {
    [Inject] GameManager gameManager;
    public ISession session { get; private set; }

    private bool IsDebug;

    public SessionManager(bool isDebug)
    {
        IsDebug = isDebug;
    }

    public event Action<string, string> PlayerJoined;
    public event Action<string, string> PlayerLeft;
    public event Action<Dictionary<string, string>, string> SessionCreated;
    public event Action PlayersReady;
    public async Awaitable StartAsync(CancellationToken cancellation = default)
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            await AuthenticationService.Instance.UpdatePlayerNameAsync("myPlayerName");
            Debug.Log($"Sign in anonymously succeeded! PlayerID: {AuthenticationService.Instance.PlayerId}, PlayerName: {AuthenticationService.Instance.PlayerName}");
            if (IsDebug) {
                var options = new SessionOptions();
                options.MaxPlayers = 2;
                options.IsPrivate = true;
                options.Name = "room";
                await StartSessionAsHost(options
                    .WithRelayNetwork("europe-central2")
                    .WithPlayerName(VisibilityPropertyOptions.Public));
                LoadScene("Game");
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    public async Task StartSessionAsHost(SessionOptions options)
    {
        gameManager.SetState(GameState.Loading);
        session = await MultiplayerService.Instance.CreateSessionAsync(options);
        gameManager.SetState(GameState.Idle);

        session.PlayerJoined += OnPlayerJoined;
        session.PlayerHasLeft += OnPlayerLeft;
        Debug.Log($"Session {session.Id} created! Join code: {session.Code}");
        SessionCreated?.Invoke(CurrentPlayers(), session.Code);

    }

    public async Task JoinSessionAsClient(string connectionString, ConnectionType connectionType)
    {
        var joinOptions = new JoinSessionOptions().WithPlayerName(VisibilityPropertyOptions.Public);
        gameManager.SetState(GameState.Loading);

        switch (connectionType) {
            case ConnectionType.ById:
                try
                {
                    session = await MultiplayerService.Instance.JoinSessionByIdAsync(connectionString, joinOptions);
                    gameManager.SetState(GameState.Idle);
                }
                catch (System.Exception e)
                {
                    gameManager.SetState(GameState.Idle);
                    Debug.LogError($"Failed to query sessions: {e}");
                }
                break;
            case ConnectionType.ByCode:
                try
                {
                    session = await MultiplayerService.Instance.JoinSessionByCodeAsync(connectionString, joinOptions);
                    gameManager.SetState(GameState.Idle);
                }
                catch (System.Exception e)
                {
                    gameManager.SetState(GameState.Idle);
                    Debug.LogError($"Failed to query sessions: {e}");
                }
                break;
        }
        gameManager.SetState(GameState.Idle);
        session.PlayerJoined += OnPlayerJoined;
        session.PlayerHasLeft += OnPlayerLeft;
        SessionCreated?.Invoke(CurrentPlayers(), session.Code);
    }
    public async Task LeaveSession()
    {
        if (session == null)
        {
            Debug.LogWarning("Попытка покинуть несуществующую сессию.");
            return;
        }

        try
        {
            gameManager.SetState(GameState.Loading);

            session.PlayerJoined -= OnPlayerJoined;
            session.PlayerHasLeft -= OnPlayerLeft;

            await session.LeaveAsync();

            Debug.Log("Сессия успешно покинута.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при выходе из сессии: {e.Message}");
        }
        finally
        {
            session = null;
            gameManager.SetState(GameState.Idle);
        }
    }
    private void OnPlayerJoined(string playerId)
    {
        Debug.Log($"{playerId} joined");
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log($"Host checking player count: {session.PlayerCount} / {session.MaxPlayers}");

            if (session.PlayerCount >= session.MaxPlayers)
            {
                Debug.Log("Lobby full!");

                PlayersReady.Invoke();
            }
        }
        PlayerJoined?.Invoke(playerId, GetPlayerName(playerId));
    }
    private void OnPlayerLeft(string playerId)
    {
        Debug.Log($"{playerId} left");
        PlayerLeft?.Invoke(playerId, "");
    }
    public string GetPlayerName(string playerId)
    {
        return session.GetPlayer(playerId).GetPlayerName();
    }
    public Dictionary<string, string> CurrentPlayers()
    {
        var result = new Dictionary<string, string>();

        foreach (var player in session.Players)
            result.Add(player.Id, player.GetPlayerName());

        return result;
    }

    public void LoadScene(string sceneName)
    {
        if (session.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
    public void CheckSession()
    {
        if (session != null)
        {
            SessionCreated?.Invoke(CurrentPlayers(), session.Code);
            if (session.PlayerCount == session.MaxPlayers) PlayersReady.Invoke();
        }
    }
}

public enum ConnectionType
{
    ById,ByCode
}