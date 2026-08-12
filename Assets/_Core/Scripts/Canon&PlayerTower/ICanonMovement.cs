using UnityEngine;

public interface ICanonMovement : IUpdatable
{
    public Vector3 CurrentPosition { get; }
}
