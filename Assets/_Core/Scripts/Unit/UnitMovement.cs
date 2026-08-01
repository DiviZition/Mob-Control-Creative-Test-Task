using R3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour, IActivatable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _initialMoveSpeed;

    private Vector3 _movingDirection;
    private float _currentMoveSpeed;

    public void Update()
    {
        if (_agent.enabled == false || _agent.isOnNavMesh == false)
            return;

        _agent.Move(_movingDirection * _currentMoveSpeed * Time.deltaTime);
    }

    public void Enable() => _agent.enabled = true;
    public void Disable() => _agent.enabled = false;

    public void RotateUnit(Quaternion newDirection)
    {
        _transform.localRotation = newDirection;
        _movingDirection = _transform.forward;
    }  

    public void WarpAgent(Vector3 position) => _agent.Warp(position);
    public void SetNewMoveSpeed(float newSpeed) => _currentMoveSpeed = newSpeed;

    public void ResetAgent()
    {
        _currentMoveSpeed = _initialMoveSpeed;
        _agent.enabled = false;
        _agent.Warp(_transform.position);
        _movingDirection = _transform.forward;
        _agent.enabled = true;
    }
}