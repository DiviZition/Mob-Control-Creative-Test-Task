using UnityEngine;
using UnityEngine.AI;
public interface IUnitMovement
{
    public void TeleportToPosition(Vector3 newPosition);
    public void SetNewMoveSpeed(float newSpeed);
    public void SetDirection(Quaternion newDirection);
    public void MoveUnitInDirection(float deltaTime);
    public void ResetToInitialState();

    public void Enable();
    public void Disable();
}

public class UnitNavMeshMovement : IUnitMovement
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _initialMoveSpeed;

    private Vector3 _movingDirection;
    private float _currentMoveSpeed;

    public UnitNavMeshMovement(NavMeshAgent unitAgent, Transform unitTransform, float baseMoveSpeed)
    {
        _agent = unitAgent;
        _transform = unitTransform;
        _initialMoveSpeed = baseMoveSpeed;
    }

    public void Enable() => _agent.enabled = true;
    public void Disable() => _agent.enabled = false;

    public void MoveUnitInDirection(float deltaTime)
    {
        if (_agent.enabled == false || _agent.isOnNavMesh == false)
            return;

        _agent.Move(_movingDirection * _currentMoveSpeed * deltaTime);
    }

    public void SetNewMoveSpeed(float newSpeed) => _currentMoveSpeed = newSpeed +Random.Range(-0.1f, 0.1f);//Random to avoid navMesh stacking

    public void TeleportToPosition(Vector3 newPosition) => _agent.Warp(newPosition);

    public void SetDirection(Quaternion newDirection)
    {
        _transform.localRotation = newDirection;
        _movingDirection = _transform.forward;
    }

    public void ResetToInitialState()
    {
        SetNewMoveSpeed(_initialMoveSpeed);
        _agent.enabled = false;
        TeleportToPosition(_transform.position);
        _agent.enabled = true;
    }
}