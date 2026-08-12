using System;

public class KeyboardInput : IInputProvider, IDisposable
{
    public float AxisHorizontal { get; private set;}
    public bool IsShooting {get; private set;}

    private MainGameInput _input;

    public KeyboardInput()
    {
        _input = new MainGameInput();
        _input.Enable();
    }

    public void Dispose()
    {
        _input.Disable();
        _input.Dispose();
    }
}
