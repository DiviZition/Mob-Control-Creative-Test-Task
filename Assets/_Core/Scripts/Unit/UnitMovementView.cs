using UnityEngine;
using UnityEngine.AI;

public class UnitMovementView : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    private Transform _tranform;
    private IUnitMovement _movementModel;

    public void Init(IUnitMovement movementModel, Transform transform)
    {
        _movementModel = movementModel;
        _tranform = transform;

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
        if (_agent.enabled == true && _movementModel.IsDisabled == false)
            _agent.Move(_movementModel.MoveDirectionVelocity);
    }

    public void Disable() => _agent.enabled = false;
    public void Enable() => _agent.enabled = true;
    private void WarpToPosition(Vector3 vector) => _agent.Warp(vector);
    private void RotateUnit(Quaternion quaternion) => _tranform.rotation = quaternion;
}
