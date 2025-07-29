using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public enum ActionMap
    {
        Player,
        UI,
        None // Need this to disable all input actions
    }

    public event EventHandler OnAttackAction;
    public event EventHandler OnJumpAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnResumeAction;
    public event EventHandler OnInventoryAction;

    private PlayerInputActions playerInputActions;
    private InputActionMap currentInputActionMap;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Attack.performed += AttackOnPerformed;
        playerInputActions.Player.Jump.performed += JumpOnPerformed;
        playerInputActions.Player.Pause.performed += PauseOnPerformed;
        playerInputActions.Player.Inventory.performed += InventoryOnPerformed;
        playerInputActions.UI.Resume.performed += ResumeOnPerformed;
    }

    private void InventoryOnPerformed(InputAction.CallbackContext context)
    {
        OnInventoryAction?.Invoke(this, EventArgs.Empty);
    }

    private void ResumeOnPerformed(InputAction.CallbackContext context)
    {
        OnResumeAction?.Invoke(this, EventArgs.Empty);
    }

    private void PauseOnPerformed(InputAction.CallbackContext context)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
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

    public void EnableActionMap(ActionMap actionMap)
    {
        currentInputActionMap?.Disable();
        var asset = playerInputActions.asset;
        currentInputActionMap = asset.FindActionMap(actionMap.ToString(), throwIfNotFound: true);
        currentInputActionMap.Enable();
    }

    public void DisableActionMap(ActionMap actionMap)
    {
        var asset = playerInputActions.asset;
        var map = asset.FindActionMap(actionMap.ToString(), throwIfNotFound: true);
        map.Disable();
    }
}