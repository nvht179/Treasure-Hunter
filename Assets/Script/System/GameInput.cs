using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : PersistentManager<GameInput>
{

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";

    public enum ActionMap
    {
        Player,
        UI,
        None // Need this to disable all input actions
    }

    public enum Binding
    {
        Move_Left,
        Move_Right,
        Jump,
        Attack,
        AttackAlternate,
        Interact,
        Inventory,
        Pause,
    }

    public event EventHandler OnAttackAction;
    public event EventHandler OnAttackAlternateAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnJumpAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnResumeAction;
    public event EventHandler OnInventoryAction;
    public event EventHandler OnBindingRebind;

    private PlayerInputActions playerInputActions;
    private InputActionMap currentInputActionMap;

    protected override void Awake()
    {
        base.Awake();

        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }

        playerInputActions.Player.Attack.performed += AttackOnPerformed;
        playerInputActions.Player.AttackAlternate.performed += AttackAlternateOnPerformed;
        playerInputActions.Player.Interact.performed += InteractOnPerformed;
        playerInputActions.Player.Jump.performed += JumpOnPerformed;
        playerInputActions.Player.Pause.performed += PauseOnPerformed;
        playerInputActions.Player.Inventory.performed += InventoryOnPerformed;
        playerInputActions.UI.Resume.performed += ResumeOnPerformed;
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        if (newState == GameManager.State.GamePlaying)
        {
            EnableActionMap(ActionMap.Player);
        }
        else
        {
            EnableActionMap(ActionMap.UI);
        }
    }

    private void InteractOnPerformed(InputAction.CallbackContext context) 
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
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
    
        private void AttackAlternateOnPerformed(InputAction.CallbackContext obj)
    {
        OnAttackAlternateAction?.Invoke(this, EventArgs.Empty);
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

        Debug.Log("Enabled action map: " + actionMap.ToString());
    }

    public void DisableActionMap(ActionMap actionMap)
    {
        var asset = playerInputActions.asset;
        var map = asset.FindActionMap(actionMap.ToString(), throwIfNotFound: true);
        map.Disable();
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Move_Left:
                return playerInputActions.Player.MoveHorizontal.bindings[1].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.MoveHorizontal.bindings[2].ToDisplayString();
            case Binding.Attack:
                return playerInputActions.Player.Attack.bindings[0].ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            case Binding.AttackAlternate:
                return playerInputActions.Player.AttackAlternate.bindings[0].ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
            case Binding.Jump:
                return playerInputActions.Player.Jump.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            case Binding.Inventory:
                return playerInputActions.Player.Inventory.bindings[0].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            default:
                return "";
        }
    }

    public void GetBindingAction(Binding binding, out InputAction inputAction, out int bindingIndex)
    {
        switch (binding)
        {
            default:
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.MoveHorizontal;
                bindingIndex = 1;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.MoveHorizontal;
                bindingIndex = 2;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
            case Binding.Jump:
                inputAction = playerInputActions.Player.Jump;
                bindingIndex = 0;
                break;
            case Binding.Attack:
                inputAction = playerInputActions.Player.Attack;
                bindingIndex = 0;
                break;
            case Binding.AttackAlternate:
                inputAction = playerInputActions.Player.AttackAlternate;
                bindingIndex = 0;
                break;
            case Binding.Inventory:
                inputAction = playerInputActions.Player.Inventory;
                bindingIndex = 0;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
        }
    }

    public void RebindBinding(Binding binding, Action<bool> onRebindFinished)
    {
        playerInputActions.Player.Disable();
        GetBindingAction(binding, out InputAction inputAction, out int bindingIndex);
        StartRebindCoroutine(inputAction, bindingIndex, onRebindFinished);
    }

    public void StartRebindCoroutine(InputAction action, int bindingIndex, Action<bool> onRebindFinished)
    {
        StartCoroutine(RebindCoroutine(action, bindingIndex, onRebindFinished));
    }

    private IEnumerator RebindCoroutine(InputAction action, int bindingIndex, Action<bool> onRebindFinished)
    {
        var backupJson = playerInputActions.SaveBindingOverridesAsJson();
        var excluded = playerInputActions.asset
            .bindings
            .Where(b => !b.isComposite && !b.isPartOfComposite)
            .Select(b => b.effectivePath)
            .ToHashSet();

        bool success = false;
        bool done = false;

        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding(string.Join(",", excluded))
            .OnComplete(op =>
            {
                op.Dispose();
                var newPath = action.bindings[bindingIndex].effectivePath;

                if (IsDuplicate(newPath, action))
                {
                    playerInputActions.LoadBindingOverridesFromJson(backupJson);
                    Debug.Log($"Duplicate binding '{newPath}' detected. Rebinding canceled.");
                    success = false;
                }
                else
                {
                    SaveOverrides();
                    success = true;
                }

                done = true;  // wake up the coroutine
            })
            .Start();

        // Wait until either success or failure has been set
        yield return new WaitUntil(() => done);

        onRebindFinished(success);
    }


    private bool IsDuplicate(string path, InputAction skipAction)
    {
        return playerInputActions.asset
            .bindings
            .Where(b => !b.isPartOfComposite)
            .Any(b => b.effectivePath == path && b.action != skipAction.name);
    }

    private void SaveOverrides()
    {
        PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

}