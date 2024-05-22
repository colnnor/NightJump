using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public partial class PlayerInputHandler : MonoBehaviour
{
    PlayerInputActions playerInput;

    #region events
    public event Action OnForwardPressed;
    public event Action OnRightPressed;
    public event Action OnBackwardPressed;
    public event Action OnLeftPressed;
    public event Action OnAnyPressed;
    public event Action OnForwardReleased;
    public event Action OnRightReleased;
    public event Action OnBackwardReleased;
    public event Action OnLeftReleased;
    public event Action OnAnyReleased;
    #endregion

    #region unity events
    public BoolEventChannel lightEventChannel;
    #endregion
    private void Start()
    {
        playerInput = new();
        playerInput.Enable();

        playerInput.Player.Forward.performed += ForwardPressed;
        playerInput.Player.Forward.canceled += ForwardReleased;
        playerInput.Player.Right.performed += RightPressed;
        playerInput.Player.Right.canceled += RightReleased;
        playerInput.Player.Backward.performed += BackwardPressed;
        playerInput.Player.Backward.canceled += BackwardReleased;
        playerInput.Player.Left.performed += LeftPressed;
        playerInput.Player.Left.canceled += LeftReleased;
        playerInput.Player.Light.performed += LightPressed;
        playerInput.Player.Light.canceled += LightCanceled;
        
    }



    #region InputPerformed
    private void LightPressed(InputAction.CallbackContext obj)
    {
        lightEventChannel.Invoke(true);
    }
    private void ForwardPressed(InputAction.CallbackContext obj)
    {
        Debug.Log("Pressed in input handler");

        OnAnyPressed?.Invoke();
        OnForwardPressed?.Invoke();
    }
    private void RightPressed(InputAction.CallbackContext obj)
    {
        OnAnyPressed?.Invoke();
        OnRightPressed?.Invoke();
    }
    private void BackwardPressed(InputAction.CallbackContext obj)
    {
        OnAnyPressed?.Invoke();
        OnBackwardPressed?.Invoke();
    }
    private void LeftPressed(InputAction.CallbackContext obj)
    {
        OnAnyPressed?.Invoke();
        OnLeftPressed?.Invoke();
    }
    #endregion

    #region InputReleased
    private void LightCanceled(InputAction.CallbackContext obj)
    {
        lightEventChannel.Invoke(false);
    }
    private void ForwardReleased(InputAction.CallbackContext obj)
    {
        OnAnyReleased?.Invoke();
        OnForwardReleased?.Invoke();
    }
    private void RightReleased(InputAction.CallbackContext obj)
    {
        OnAnyReleased?.Invoke();
        OnRightReleased?.Invoke();
    }

    private void BackwardReleased(InputAction.CallbackContext obj)
    {
        OnAnyReleased?.Invoke();
        OnBackwardReleased?.Invoke();
    }
    private void LeftReleased(InputAction.CallbackContext obj)
    {
        OnAnyReleased?.Invoke();
        OnLeftReleased?.Invoke();
    }
    #endregion

}
