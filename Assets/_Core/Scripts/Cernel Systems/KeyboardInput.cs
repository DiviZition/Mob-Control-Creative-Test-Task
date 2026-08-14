using System;
using System.Diagnostics;
using UnityEngine.InputSystem;

public class KeyboardInput : IInputProvider
{
    public float AxisHorizontal { get; private set;}
    public bool IsShooting {get; private set;}

    private MainGameInput _input;

    public KeyboardInput()
    {
        _input = new MainGameInput();
        _input.Enable();

        _input.Player.AxisHorizontal.performed += ReadAxisHorizontalinput;
        _input.Player.AxisHorizontal.canceled += ReadAxisHorizontalinput;

        _input.Player.Shoot.started += ReadShootInput;
        _input.Player.Shoot.canceled += ReadShootInput;
    }

    private void ReadShootInput(InputAction.CallbackContext context) => IsShooting = context.ReadValue<float>() > 0;
    private void ReadAxisHorizontalinput(InputAction.CallbackContext context) => AxisHorizontal = context.ReadValue<float>();

    public void Dispose()
    {
        _input.Player.AxisHorizontal.performed -= ReadAxisHorizontalinput;
        _input.Player.Shoot.performed -= ReadShootInput;

        _input.Disable();
        _input.Dispose();
    }
}
