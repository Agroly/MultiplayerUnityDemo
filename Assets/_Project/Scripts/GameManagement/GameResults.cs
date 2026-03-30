using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using VContainer;


public class GameResults : MonoBehaviour
{
    [Inject] GameManager gameManager;
    [Inject] SessionManager sessionManager;
    [Inject] PlayerSpawner spawner;

    [SerializeField] TextMeshProUGUI winnerTextUI;
    public void ShowResults(string winnerId)
    {
        this.winnerTextUI.gameObject.SetActive(true);
        var winnerName = sessionManager.GetPlayerName(winnerId);
        this.winnerTextUI.text = $"Победитель: {winnerName}";
        gameManager.SetState(GameState.End);
        spawner.DespawnPlayers();
        Invoke("BackToMenu", 5f);
    }
    private void BackToMenu()
    {
        sessionManager.LoadScene("Menu");
    }
}

