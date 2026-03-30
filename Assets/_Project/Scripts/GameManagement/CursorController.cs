using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CursorController : IStartable, IDisposable
{
    private readonly GameManager _gameManager;

    [Inject]
    public CursorController(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Start()
    {
        _gameManager.OnGameStateChanged += CheckCursor;
    }

    public void Dispose()
    {
        _gameManager.OnGameStateChanged -= CheckCursor;
    }

    private void CheckCursor(GameState gameState)
    {
        if (gameState == GameState.InGame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}