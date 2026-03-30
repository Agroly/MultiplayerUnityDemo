using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInput : IDisposable,IPlayerInput
{
    private readonly InputActions _actions;  
    private Vector2 _move;

    public float Pitch => -_move.y; 
    public float Roll => _move.x; 

    public PlayerInput()
    {
        _actions = new InputActions();
        _actions.Enable();
        _actions.Gameplay.Enable();

        _actions.Gameplay.Move.performed += OnMove;
        _actions.Gameplay.Move.canceled += OnMove;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _move = ctx.ReadValue<Vector2>();
    }


    public void Dispose()
    {
        _actions.Gameplay.Move.performed -= OnMove;
        _actions.Gameplay.Move.canceled -= OnMove;

        _actions.Dispose();
    }
}
