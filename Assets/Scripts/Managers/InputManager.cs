using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private InputActions _inputActions;
    public event EventHandler OnJumpPerformed;
    public event EventHandler OnRunPerformed;
    public event EventHandler OnInteractPerformed;
    public event EventHandler OnAttackPerformed;
    public event EventHandler OnPausePerformed;
    public event EventHandler OnSwitchCamera;


    private void Awake()
    {
        Time.timeScale = 1;

        _inputActions = new InputActions();
        _inputActions.Player.Enable();
        _inputActions.Camera.Enable();
        _inputActions.UI.Enable();

        _inputActions.Player.Jump.performed += Jump_performed;
        _inputActions.Player.Run.performed += Run_performed;
        _inputActions.Player.Interact.performed += Interact_performed;
        _inputActions.Player.Attack.performed += Attack_performed;

        _inputActions.UI.Pause.performed += Pause_performed;

        _inputActions.Camera.SwitchCamera.performed += SwitchCamera_performed;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Jump.performed -= Jump_performed;
        _inputActions.Player.Interact.performed -= Interact_performed;

        _inputActions.Dispose();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Run_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnRunPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackPerformed?.Invoke(this, EventArgs.Empty);
    }
    public void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPausePerformed?.Invoke(this, EventArgs.Empty);
    }
    public void SwitchCamera_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSwitchCamera?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 moveVector = _inputActions.Player.Move.ReadValue<Vector2>();
        moveVector = moveVector.normalized;
        return moveVector;
    }

    public Vector2 GetCameraRotate()
    {
        Vector2 cameraVector = _inputActions.Camera.Rotate.ReadValue<Vector2>();
        return cameraVector;
    }




    //public Vector2 GetMouseScroll()
    //{
    //    // TODO: Get scrollUp and scrollDown, ReadValue<float> may help;
    //    // Retuen a new Vector2 type
    //    float scrollUp = _inputActions.Camera.ScrollUp.ReadValue<float>();
    //    float scrollDown = _inputActions.Camera.ScrollDown.ReadValue<float>();
    //    return new Vector2(scrollUp, scrollDown);
    //}



}
