using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnAttackAction;

    public event EventHandler OnJumpAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Attack.performed += AttackOnPerformed;
        playerInputActions.Player.Jump.performed += JumpOnPerformed;
    }

    private void JumpOnPerformed(InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void AttackOnPerformed(InputAction.CallbackContext obj)
    {
        OnAttackAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        var inputVector = playerInputActions.Player.MoveHorizontal.ReadValue<Vector2>();
        inputVector.Normalize();
        return inputVector;
    }
}