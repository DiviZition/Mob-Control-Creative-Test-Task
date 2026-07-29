using R3;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour, IDisablable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _moveSpeed;

    public void Update()
    {
        if (_agent.enabled == false || _agent.isOnNavMesh == false)
            return;

        _agent.Move(_transform.forward * _moveSpeed * Time.deltaTime);
    }

    public void Enable() => _agent.enabled = true;
    public void Disable() => _agent.enabled = false;

    public void RotateUnit(Quaternion newDirection) => _transform.localRotation = newDirection;

    public void WarpAgent(Vector3 position) => _agent.Warp(position);

    public void ResetAgent()
    {
        _agent.enabled = false;
        _agent.Warp(_transform.position);
        _agent.enabled = true;
    }
}