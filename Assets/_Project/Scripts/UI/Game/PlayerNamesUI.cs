using TMPro;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using System.Text; // Для быстрой сборки строк

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerNamesUI : MonoBehaviour
{
    [Inject] private SessionManager sessionManager;
    [Inject] private GameManager gameManager;

    private TextMeshProUGUI text;

    private readonly Dictionary<string, (string Name, int Score)> _playerData = new();

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        gameManager.OnGameStateChanged += InitPlayerNames;
        InitPlayerNames(gameManager.State);
    }

    private void InitPlayerNames(GameState gameState)
    {
        text.text = "";
        _playerData.Clear();

        if (gameState == GameState.InGame)
        {
            var players = sessionManager.CurrentPlayers();
            foreach (var player in players)
            {
                // Очищаем имя один раз при инициализации
                string cleanName = player.Value.Split('#')[0];
                _playerData[player.Key] = (cleanName, 0);
            }
            RefreshUI();
        }
    }

    public void UpdatePlayerScore(string playerId, int newScore)
    {
        Debug.Log(playerId+" "+ newScore.ToString());
        if (_playerData.ContainsKey(playerId))
        {
            var data = _playerData[playerId];
            _playerData[playerId] = (data.Name, newScore);

            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var entry in _playerData.Values)
        {
            sb.AppendLine($"{entry.Name}: {entry.Score}");
        }

        text.text = sb.ToString();
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= InitPlayerNames;
    }
}