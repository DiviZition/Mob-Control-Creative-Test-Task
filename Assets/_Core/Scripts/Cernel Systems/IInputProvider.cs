using System;

public interface IInputProvider : IDisposable
{
    float AxisHorizontal { get; }
    bool IsShooting { get; }
}