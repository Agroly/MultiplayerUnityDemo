using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

public class CountDownController : MonoBehaviour
{
    [Inject] GameManager gameManager;
    [SerializeField] TextMeshProUGUI CountDownText;

    public event Action GameStarted;

    private void Awake()
    {
        if (CountDownText != null) CountDownText.gameObject.SetActive(false);

        gameManager.OnGameStateChanged += StartCountDown;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= StartCountDown;
    }

    private void StartCountDown(GameState gameState)
    {
        if (gameState == GameState.InGame)
        {
            StartCoroutine(CountdownRoutine(3));
        }
    }

    private IEnumerator CountdownRoutine(int seconds)
    {
        if (CountDownText != null) CountDownText.gameObject.SetActive(true);

        while (seconds > 0)
        {
            if (CountDownText != null)
            {
                CountDownText.text = seconds.ToString();
            }

            yield return new WaitForSeconds(1f);
            seconds--;
        }

        if (CountDownText != null)
        {
            CountDownText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            CountDownText.gameObject.SetActive(false);
        }
        GameStarted?.Invoke();
    }
}