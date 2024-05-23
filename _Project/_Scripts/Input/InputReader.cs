using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader", order = 1)]
public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions, PlayerInputActions.IUIActions
{
    private InputAction navigateAction;
    private PlayerInputActions inputActions;
    private InputActionMap currentActionMap;
    private bool playerCanMove;

    public PlayerInputActions GetInputActions => inputActions;

    public bool SpacePressed { get; private set;}

    #region Player 
    public event UnityAction PauseEvent;
    public event UnityAction ForwardEvent;
    public event UnityAction RightEvent;
    public event UnityAction BackwardEvent;
    public event UnityAction LeftEvent;
    public event UnityAction<bool> LightEnabledEvent;
    public event UnityAction AnyPressed;
    #endregion
    #region UI
    public event UnityAction<Vector2> OnNavigate;
    #endregion
    
    void OnNavigatePerformed(InputAction.CallbackContext context)
    {
        var direction = context.ReadValue<Vector2>();
        var normalizedDirection = new Vector3(Mathf.Round(direction.x), Mathf.Round(direction.y));
        OnNavigate?.Invoke(normalizedDirection);
    }
    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.Player.SetCallbacks(this);
        }

        navigateAction = inputActions.UI.Navigate;

        inputActions.Enable();
    }

    public void SwitchActionMap(InputActionMap newMap)
    {
        currentActionMap?.Disable();
        currentActionMap = newMap;
        currentActionMap.Enable();

        if (newMap.name == "UI")
            navigateAction.performed += OnNavigatePerformed;
        
    }

    public void EnablePlayerMovement(bool enabled = true)
    {
        playerCanMove = enabled;
    }
    public void EnableInputActions(bool enabled = true)
    {
        if (enabled)
            inputActions.Enable();
        else
            inputActions.Disable();
    }

    private void OnDisable() => inputActions.Disable();

    #region onevents
    public void OnBackward(InputAction.CallbackContext context)
    {
        Debug.Log(playerCanMove);
        if(!playerCanMove) return;
        if(context.phase == InputActionPhase.Performed)
        {
            AnyPressed?.Invoke();
            BackwardEvent?.Invoke();
        }
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if(!playerCanMove) return;
        if(context.phase == InputActionPhase.Performed)
        {
            AnyPressed?.Invoke();
            ForwardEvent?.Invoke();
        }
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if(!playerCanMove) return;
        if(context.phase == InputActionPhase.Performed)
        {
            AnyPressed?.Invoke();
            LeftEvent?.Invoke();
        }
    }

    public void OnLight(InputAction.CallbackContext context)
    {
        
        if(!playerCanMove) return;
        if(context.phase == InputActionPhase.Started)
        {
            SpacePressed = true;
            LightEnabledEvent?.Invoke(true);
        }
        
        else if (context.phase == InputActionPhase.Canceled)
        {
            SpacePressed = false;   
            LightEnabledEvent?.Invoke(false);
        }
    }

    public void OnRight(InputAction.CallbackContext context)
    {
        if(!playerCanMove) return;
        if(context.phase == InputActionPhase.Performed)
        {
            AnyPressed?.Invoke();
            RightEvent?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
        }
    }

    void PlayerInputActions.IUIActions.OnNavigate(InputAction.CallbackContext context) => throw new System.NotImplementedException();
    public void OnSubmit(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnCancel(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnPoint(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnClick(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnScrollWheel(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnMiddleClick(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnRightClick(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) => Debug.Log("");//noop
    public void OnExit(InputAction.CallbackContext context) => PauseEvent?.Invoke();
    #endregion
}
