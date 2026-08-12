using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class UnitMovementView : MonoBehaviour
{
    private IUnitMovement _movementModel;
    private NavMeshAgent _agent;
    private Transform _tranform;

    public void Init(IUnitMovement movementModel)
    {
        _movementModel = movementModel;

        _movementModel.OnChangeDirection += RotateUnit;
        _movementModel.OnTeleportToPosition += WarpToPosition;
    }

    private void OnDestroy()
    {
        _movementModel.OnChangeDirection -= RotateUnit;
        _movementModel.OnTeleportToPosition -= WarpToPosition;
    }

    private void FixedUpdate()
    {
        if (_movementModel.IsDisabled == false)
            _agent.Move(_movementModel.MoveDirectionVelocity);
    }

    private void WarpToPosition(Vector3 vector) => _agent.Warp(vector);
    private void RotateUnit(Quaternion quaternion) => _tranform.localRotation = quaternion;
}

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

public interface ILockable
{
    public bool IsDisabled { get; }
    public void ForceRemoveAllLockers();
    public void Enable();
    public void Disable();
}

public class Disablable : ILockable
{
    private byte _lockersCount;

    public bool IsDisabled => _lockersCount > 0;

    /// <summary>
    /// Just cleans the lockers list, so IsDisabled will become false anyways
    /// </summary>
    public virtual void ForceRemoveAllLockers() => _lockersCount = 0;

    /// <summary>
    /// Removes the locker from the lockers list. IsDisabled will become false, only when there are no lockers left.
    /// </summary>
    public virtual void Enable() => _lockersCount = Math.Clamp(_lockersCount, (byte)0, byte.MaxValue);

    /// <summary>
    /// Locks IsDisabled with one locker. IsDisabled will stay in true state until at least 1 loker is registered
    /// </summary>
    public virtual void Disable() => _lockersCount++;
}

public class UnitMovementModel : Disablable, IUnitMovement
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