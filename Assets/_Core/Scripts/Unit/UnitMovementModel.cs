using System;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IUnitMovement : ILockable
{
    public Vector3 MoveDirectionVelocity { get; }

    public void TeleportToPosition(Vector3 newPosition);
    public void SetNewMoveSpeed(float newSpeed);
    public void SetDirection(Quaternion newDirection);
    public void MoveUnitInDirection(float deltaTime);
    public void ResetToInitialState();

    public event Action<Vector3> OnTeleportToPosition;
    public event Action<Quaternion> OnChangeDirection;
}

public class UnitMovementModel : Lockable, IUnitMovement
{
    [SerializeField] private float _initialMoveSpeed;

    private Vector3 _movingDirection;
    private float _currentMoveSpeed;

    public Vector3 MoveDirectionVelocity {  get; private set; }
    public event Action<Vector3> OnTeleportToPosition;
    public event Action<Quaternion> OnChangeDirection;

    public UnitMovementModel(float baseMoveSpeed) => _initialMoveSpeed = baseMoveSpeed;

    public void MoveUnitInDirection(float deltaTime)
    {
        if (IsDisabled == true)
            return;

        MoveDirectionVelocity = _movingDirection * _currentMoveSpeed * deltaTime;
    }

    public void ResetToInitialState() => SetNewMoveSpeed(_initialMoveSpeed);
    public void SetNewMoveSpeed(float newSpeed) => _currentMoveSpeed = newSpeed + Random.Range(-0.1f, 0.1f);//Random to avoid navMesh stacking
    public void TeleportToPosition(Vector3 newPosition) => OnTeleportToPosition?.Invoke(newPosition);
    public void SetDirection(Quaternion newDirection)
    {
        OnChangeDirection?.Invoke(newDirection);
        _movingDirection = newDirection * Vector3.forward;
    }

}